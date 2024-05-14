using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Curiosity.FileDataReaderWriters.Readers;
using Sylvan.Data.Csv;

namespace Curiosity.FileDataReaderWriters.SylvanCsv;

/// <summary>
/// Класс для построчного чтения CSV файлов. 
/// </summary>
/// <inheritdoc cref="IFileDataReader"/>
public class CsvFileDataReader : IFileDataReader
{
    /// <summary>
    /// Максимальное количество пустых строк подряд, после которых мы перестаем читать файл.
    /// </summary>
    private const int MaxSequentialEmptyRowsCount = 10;

    private readonly StreamReader _streamReader;
    private readonly CsvDataReader _csvReader;

    private readonly Stream _fileStream;
    private readonly bool _disposeFileStream;

    private int _rIdx;
    private int _sequentialEmptyRowsCount;
    private readonly int _startRowIdx;

    private IReadOnlyList<string?>? _currentRow;
    private readonly List<string?> _rowStringBuffer;
    private object[] _rowObjectBuffer;

    /// <inheritdoc />
    public int MaxSupportedRowsCount => int.MaxValue;

    /// <inheritdoc />
    /// <remarks>
    /// Уменьшаем на 1, потому что после чтения мы всегда увеличиваем <see cref="_rIdx"/> после чтения очередной строки.
    /// </remarks>
    public int CurrentRowIdx => _rIdx - 1;

    private CsvFileDataReader(
        Stream fileStream,
        bool disposeFileStream,
        int? columnsCount,
        Encoding encoding,
        char delimiter,
        int startRowIdx)
    {
        _fileStream = fileStream;
        _disposeFileStream = disposeFileStream;

        _streamReader = new StreamReader(fileStream, encoding, false, 128 * 1024);
        _csvReader = CsvDataReader.Create(
            _streamReader,
            new CsvDataReaderOptions
            {
                Delimiter = delimiter,
                HasHeaders = false, // мы обычно сами парсим заголовки, поэтому нет смысла что-то там парсить
            });

        _rowStringBuffer = columnsCount.HasValue
            ? new List<string?>(columnsCount.Value)
            : new List<string?>();
        // если что, массив может быть пересоздан, если мы не знаем точное количество колонок
        _rowObjectBuffer = new object[columnsCount ?? 256];

        _rIdx = 1;
        _startRowIdx = Math.Max(_rIdx, startRowIdx);
        _sequentialEmptyRowsCount = 0;
    }

    /// <summary>
    /// Создает готовый к работе <see cref="CsvFileDataReader"/>.
    /// </summary>
    /// <param name="fileStream">Поток с данными файла.</param>
    /// <param name="columnsCount">Количество колонок в CSV файле. Если не указано, то количество колонок в каждой строке будет определяться автоматически.</param>
    /// <param name="startRowIdx">Номер строки, начиная с которой надо читать файл.</param>
    /// <param name="fieldDelimiter">Разделитель колонок.</param>
    /// <param name="encoding">Кодировка файлов. Если не указано используем UTF-8</param>
    /// <param name="disposeFileStream">Нужно ли освобождать <paramref name="fileStream"/> после завершения чтения.</param>
    public static CsvFileDataReader CreateReader(
        Stream fileStream,
        int? columnsCount = null,
        int startRowIdx = 1,
        char fieldDelimiter = ';',
        Encoding? encoding = null,
        bool disposeFileStream = false)
    {
        Guard.AssertNotNull(fileStream, nameof(fileStream));
        Guard.AssertNotNegative(columnsCount, nameof(columnsCount));
        Guard.AssertNotNegative(startRowIdx, nameof(startRowIdx));

        return new CsvFileDataReader(
            fileStream,
            disposeFileStream,
            columnsCount,
            encoding ?? Encoding.UTF8,
            fieldDelimiter,
            startRowIdx);
    }

    /// <inheritdoc />
    public bool Read()
    {
        while (true)
        {
            if (_rIdx > MaxSupportedRowsCount) return false;
            if (_sequentialEmptyRowsCount > MaxSequentialEmptyRowsCount) return false;

            _rowStringBuffer.Clear();
            var isEmptyRow = true;
            if (_csvReader.Read())
            {
                // пропускаем заголовки и прочее
                if (_rIdx >= _startRowIdx)
                {
                    // количество ячеек в строке может меняться
                    // до начала работ мы не можем знать, сколько их там,
                    // поэтому адаптируем размер буфера для чтения 
                    var factColumnsCount = _csvReader.RowFieldCount;
                    if (factColumnsCount > _rowObjectBuffer.Length)
                    {
                        _rowObjectBuffer = new object[factColumnsCount];
                    }

                    var columnsCount = _csvReader.GetValues(_rowObjectBuffer);
                    for (var cIdx = 0; cIdx < columnsCount; cIdx++)
                    {
                        var cellValue = _rowObjectBuffer[cIdx].ToString()?.Trim();

                        if (String.IsNullOrWhiteSpace(cellValue))
                        {
                            _rowStringBuffer.Add(null);
                        }
                        else
                        {
                            isEmptyRow = false;
                            _rowStringBuffer.Add(cellValue);
                        }
                    }
                }
            }

            _rIdx++;

            if (isEmptyRow)
            {
                _sequentialEmptyRowsCount++;
                _currentRow = null;

                // если прочитали пустую строку, попробуем прочитать еще одну строку,
                // чтобы упростить клиентский код и не добавлять в него парсинг пустых строк

                continue;
            }

            // если какие-то данные были, сбросим счетчик пустых строк
            _sequentialEmptyRowsCount = 0;

            // сохраним данные, чтобы вернуть по требованию
            _currentRow = _rowStringBuffer;

            return true;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<string?>? GetRow() => _currentRow;

    /// <inheritdoc />
    public void Dispose()
    {
        _csvReader.Dispose();
        _streamReader.Dispose();

        if (_disposeFileStream)
            _fileStream.Dispose();
    }
}
