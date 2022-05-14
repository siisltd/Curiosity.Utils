using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Curiosity.Tools;
using Curiosity.Tools.TempFiles;

namespace Curiosity.Archiver.SharpZip
{
    /// <summary>
    /// SharpZib archiver.
    /// </summary>
    public class SharpZipArchiver : IArchiver
    {
        /// <summary>
        /// Size of buffer for reading data from file.
        /// </summary>
        /// <remarks>
        /// About better buffer size: https://www.javamex.com/tutorials/io/input_stream_buffer_size.shtml
        /// </remarks>
        private const int BufferSize = 32768;

        /// <summary>
        /// Level of ZIP compression.
        /// </summary>
        /// <remarks>
        /// 0-9, 9 being the highest level of compression
        /// </remarks>
        private const int ZipLevel = 9;

        private readonly ArrayPool<byte> _arrayPool;

        private readonly TempFileOptions _tempFileOptions;
        private readonly ITempFileStreamFactory _tempFileStreamFactory;

        /// <inheritdoc cref="SharpZipArchiver"/>
        public SharpZipArchiver(
            ITempFileStreamFactory tempFileStreamFactory,
            TempFileOptions tempFileOptions)
        {
            _tempFileStreamFactory = tempFileStreamFactory;
            _tempFileOptions = tempFileOptions;
            _arrayPool = ArrayPool<byte>.Create();
        }

        /// <inheritdoc />
        public Task<TempFileStream> ZipDirAsync(
            string dirToCompressPath,
            bool useZip64 = true,
            CancellationToken cts = default)
        {
            if (String.IsNullOrWhiteSpace(dirToCompressPath))
                throw new ArgumentNullException(nameof(dirToCompressPath));

            if (!Directory.Exists(dirToCompressPath))
                throw new ArgumentException($"Directory does not exist: \"{dirToCompressPath}\"'");

            var zipFileName = $"{Guid.NewGuid()}.zip";
            var archivePath = Path.Combine(_tempFileOptions.TempPath, zipFileName);
            try
            {
                var fastZip = new FastZip
                {
                    UseZip64 = useZip64 ? UseZip64.On : UseZip64.Off,
                    RestoreDateTimeOnExtract = true,
                    RestoreAttributesOnExtract = true,
                    CompressionLevel = Deflater.CompressionLevel.BEST_COMPRESSION
                };

                fastZip.CreateZip(archivePath, dirToCompressPath, true, (string) null!, null);
                return Task.FromResult(new TempFileStream(archivePath, FileMode.Open));
            }
            catch
            {
                if (File.Exists(archivePath))
                {
                    File.Delete(archivePath);
                }

                throw;
            }
        }

        /// <inheritdoc />
        public Task<TempFileStream> ZipFilesToStreamAsync(
            IList<string> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            IList<string>? zipFileNames = null,
            CancellationToken cts = default)
        {
            if (sourceFiles == null) throw new ArgumentNullException(nameof(sourceFiles));

            var sourceFileNames = Concatenate(sourceFiles, zipFileNames);

            return ZipFilesToStreamAsync(sourceFileNames, useZip64, zipFileName, cts);
        }

        /// <inheritdoc />
        public async Task<TempFileStream> ZipFilesToStreamAsync(
            IReadOnlyList<FileNames> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            CancellationToken cts = default)
        {
            if (sourceFiles == null) throw new ArgumentNullException(nameof(sourceFiles));

            var tempStream = _tempFileStreamFactory.CreateTempFileStream(GetZipFileName(zipFileName));
            var buffer = _arrayPool.Rent(BufferSize);

            try
            {
                using (var zipStream = new ZipOutputStream(tempStream))
                {
                    zipStream.IsStreamOwner = false;
                    await ZipFilesAsync(buffer, zipStream, sourceFiles, useZip64, cts);
                }

                // we need to set position to start because temp stream can be used in another places
                tempStream.Position = 0;
                return tempStream;
            }
            catch
            {
                _arrayPool.Return(buffer);
                tempStream?.Dispose();
                throw;
            }
        }

        private string GetZipFileName(string? zipFileName = null)
        {
            return String.IsNullOrWhiteSpace(zipFileName)
                ? $"{Guid.NewGuid()}.zip"
                : zipFileName;
        }

        private async Task ZipFilesAsync(
            byte[] buffer,
            ZipOutputStream zipStream,
            IReadOnlyList<FileNames> sourceFiles,
            bool useZip64 = true,
            CancellationToken cts = default)
        {
            zipStream.SetLevel(ZipLevel);
            zipStream.UseZip64 = useZip64 ? UseZip64.On : UseZip64.Off;

            var entryNames = new HashSet<string>();
            for (var i = 0; i < sourceFiles.Count; i++)
            {
                var sourceFileName = sourceFiles[i].StorageFileName;
                var entryFileName = sourceFiles[i].UserFileName;

                // to avoid file name duplication in the archive
                if (entryNames.Contains(entryFileName))
                {
                    var ix = 1;
                    while (entryNames.Contains($"{entryFileName} {ix}"))
                    {
                        ix++;
                    }
                    entryFileName = $"{entryFileName} {ix}";
                }
                
                entryNames.Add(entryFileName);

                // archiving
                var fileInfo = new FileInfo(sourceFileName);
                var entry = new ZipEntry(ZipEntry.CleanName(entryFileName))
                {
                    DateTime = fileInfo.LastWriteTime, // Note the zip format stores 2 second granularity
                    Size = fileInfo.Length,
                    IsUnicodeText = true
                };
                zipStream.PutNextEntry(entry);

                using (var streamReader = File.OpenRead(sourceFileName))
                {
                    await streamReader.CopyToAsync(zipStream, buffer, cts);
                }
            }

            zipStream.Finish();
            zipStream.Close();
        }

        /// <inheritdoc />
        [Obsolete("Use method with file names collection")]
        public Task<string> ZipFilesToFileAsync(
            IList<string> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            IList<string>? zipFileNames = null,
            CancellationToken cts = default)
        {
            if (sourceFiles == null) throw new ArgumentNullException(nameof(sourceFiles));

            var sourceFileNames = Concatenate(sourceFiles, zipFileNames);

            return ZipFilesToFileAsync(sourceFileNames, useZip64, zipFileName, cts);
        }

        /// <inheritdoc />
        public async Task<string> ZipFilesToFileAsync(
            IReadOnlyList<FileNames> sourceFiles,
            bool useZip64 = true,
            string? zipFileName = null,
            CancellationToken cts = default)
        {
            if (sourceFiles == null) throw new ArgumentNullException(nameof(sourceFiles));

            var zipFilePath = Path.Combine(_tempFileOptions.TempPath, GetZipFileName(zipFileName));
            var buffer = _arrayPool.Rent(BufferSize);

            try
            {
                using (var zipStream = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    await ZipFilesAsync(buffer, zipStream, sourceFiles, useZip64, cts);
                }

                return zipFilePath;
            }
            catch
            {
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                _arrayPool.Return(buffer);
                throw;
            }
        }

        private static IReadOnlyList<FileNames> Concatenate(IList<string> sourceFiles, IList<string>? zipFileNames)
        {
            if (zipFileNames != null && zipFileNames.Count != sourceFiles.Count)
                throw new ArgumentException($"Items count in {nameof(sourceFiles)} and {nameof(zipFileNames)} must be equal");

            var sourceFileNames = new FileNames[sourceFiles.Count];
            for (var i = 0; i < sourceFiles.Count; i++)
            {
                sourceFileNames[i] = zipFileNames == null
                    ? new FileNames(sourceFiles[i])
                    : new FileNames(sourceFiles[i], zipFileNames[i]);
            }

            return sourceFileNames;
        }

        /// <inheritdoc />
        public string UnzipFile(FileStream file, string? unzipDirectoryPath = null)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            // prepare directory for unzip files
            var tempDirectoryPath = String.IsNullOrWhiteSpace(unzipDirectoryPath)
                ? Path.Combine(_tempFileOptions.TempPath, "unzip", UniqueKeyGenerator.GenerateRandomKey(10))
                : unzipDirectoryPath;
            Directory.CreateDirectory(tempDirectoryPath);

            // unzip
            var fastZip = new FastZip();
            fastZip.ExtractZip(file, tempDirectoryPath, FastZip.Overwrite.Never, null, null, null, true, false);

            return tempDirectoryPath;
        }
    }
}
