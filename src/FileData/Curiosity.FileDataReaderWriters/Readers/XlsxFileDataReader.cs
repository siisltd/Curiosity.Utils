using Curiosity.FileDataReaderWriters.Helpers;
using Curiosity.FileDataReaderWriters.Resources;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using global::System;
using global::System.Collections.Generic;
using global::System.IO;

namespace Curiosity.FileDataReaderWriters.Readers;

/// <summary>
/// Класс для построчного чтения данных их Xlsx/xlsx файлов.
/// </summary>
/// <inheritdoc cref="IFileDataReader"/>
public class XlsxFileDataReader : IFileDataReader
{
    /// <summary>
    /// Максимальное количество пустых строк подряд, после которых мы перестаем читать файл.
    /// </summary>
    private const int MaxSequentialEmptyRowsCount = 10;

    private readonly Stream _fileStream;
    private readonly bool _disposeFileStream;

    private XlsFile? _xls;
    private readonly int? _columnsCount;
    private readonly int _maxColumnsCount;

    private int _rIdx;
    private int _sequentialEmptyRowsCount;

    private IReadOnlyList<string?>? _currentRow;
    private readonly List<string?> _buffer;

    /// <inheritdoc />
    public int MaxSupportedRowsCount => ExcelConstants.XlsxRowsMax;

    /// <inheritdoc />
    /// <remarks>
    /// Уменьшаем на 1, потому что после чтения мы всегда увеличиваем <see cref="_rIdx"/> после чтения очередной строки.
    /// </remarks>
    public int CurrentRowIdx => _rIdx - 1;

    private XlsxFileDataReader(
        Stream fileStream,
        bool disposeFileStream,
        XlsFile xls,
        int? columnsCount,
        int maxColumnsCount,
        int startRowIdx)
    {
        Guard.AssertNotNull(fileStream, nameof(fileStream));
        Guard.AssertNotNull(xls, nameof(xls));

        _fileStream = fileStream;
        _disposeFileStream = disposeFileStream;

        _xls = xls;
        _columnsCount = columnsCount;
        _maxColumnsCount = maxColumnsCount;

        _buffer = columnsCount.HasValue
            ? new List<string?>(columnsCount.Value)
            : new List<string?>();

        _rIdx = Math.Max(1, startRowIdx);
        _sequentialEmptyRowsCount = 0;
    }

    /// <summary>
    /// Создает готовый к работе <see cref="CsvFileDataReader"/>.
    /// </summary>
    /// <param name="fileStream">Поток с данными файла.</param>
    /// <param name="columnsCount">Количество колонок в Excel файле. Если не указано, то количество колонок в каждой строке будет определяться автоматически.</param>
    /// <param name="startRowIdx">Номер строки, начиная с которой надо читать файл.</param>
    /// <param name="disposeFileStream">Нужно ли освобождать <paramref name="fileStream"/> после завершения чтения.</param>
    public static XlsxFileDataReader CreateReader(
        string fileName,
        Stream fileStream,
        int? columnsCount = null,
        int startRowIdx = 1,
        bool disposeFileStream = false)
    {
        Guard.AssertNotNull(fileStream, nameof(fileStream));
        Guard.AssertNotEmpty(fileName, nameof(fileName));
        Guard.AssertNotNegative(columnsCount, nameof(columnsCount));
        Guard.AssertNotNegative(startRowIdx, nameof(startRowIdx));

        var xls = new XlsFile();

        TFileFormats fileFormat;
        int maxColumnsCount;
        var fileExtension = Path.GetExtension(fileName).ToLower();
        switch (fileExtension)
        {
            case ".xls":
                fileFormat = TFileFormats.Xls;
                maxColumnsCount = ExcelConstants.XlsColMax;
                break;
            case ".xlsx":
                fileFormat = TFileFormats.Xlsx;
                maxColumnsCount = ExcelConstants.XlsxColMax;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileName), fileName,
                    LNG.Get("Файл с расширением \"{0}\" не является Excel (.xls/.xlsx)", fileExtension));
        }

        ColumnImportType[]? columnFormats = null;
        if (columnsCount != null)
        {
            columnFormats = new ColumnImportType[columnsCount.Value];
            for (var i = 0; i < columnsCount; i++)
            {
                columnFormats[i] = ColumnImportType.Text;
            }
        }

        // для xlsx пофиг на разделитель, просто сигнатура метода кривая,
        // надо хоть что-то указать
        var fieldDelimiter = ',';
        xls.Open(fileStream, fileFormat, fieldDelimiter, 1, 1, columnFormats);

        // выбираем первый лист
        if (xls.SheetCount < 1)
            throw new ArgumentException(LNG.Get("Файл должен содержать хотя бы один лист"));

        xls.ActiveSheet = 1;

        return new XlsxFileDataReader(
            fileStream,
            disposeFileStream,
            xls,
            columnsCount,
            maxColumnsCount,
            startRowIdx);
    }

    /// <inheritdoc />
    public bool Read()
    {
        while (true)
        {
            if (_rIdx > MaxSupportedRowsCount) return false;
            if (_sequentialEmptyRowsCount > MaxSequentialEmptyRowsCount) return false;

            _buffer.Clear();
            var isEmptyRow = true;

            // используется, если мы не знаем количество колонок на момент чтения файла
            var emptyCells = 0;
            const int maxSequenceEmptyCellsCount = 10;

            // условия выхода из цикла ниже в первых строках
            for (var cIdx = 1; ; cIdx++)
            {
                // если знаем количество колонок и вышли за границы, то выходим
                if (_columnsCount.HasValue && cIdx > _columnsCount.Value) break;
                // если не знаем количество колонок и много пустых ячеек подряд, то выходим
                if (!_columnsCount.HasValue && emptyCells >= maxSequenceEmptyCellsCount) break;
                // если вышли за граница файла, то тоже выходим
                if (cIdx >= _maxColumnsCount) break;

                var cellValue = _xls!.GetTrimmedStringFromCell(_rIdx, cIdx);
                if (cellValue.Length == 0)
                {
                    emptyCells++;
                    _buffer.Add(null);
                }
                else
                {
                    isEmptyRow = false;
                    emptyCells = 0;
                    _buffer.Add(cellValue);
                }
            }

            _rIdx++;

            if (isEmptyRow)
            {
                _sequentialEmptyRowsCount++;
                _currentRow = null;

                // если прочитали пустую строку, попробуем прочитать еще одну строку,
                // чтобы упростить клиентский код
                continue;
            }

            // если какие-то данные были, сбросим счетчик пустых строк
            _sequentialEmptyRowsCount = 0;

            // сохраним данные, чтобы вернуть по требованию
            _currentRow = _buffer;

            return true;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<string?>? GetRow() => _currentRow;

    /// <inheritdoc />
    public void Dispose()
    {
        _xls = null!;

        if (_disposeFileStream)
            _fileStream.Dispose();
    }
}
