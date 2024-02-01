using Curiosity.Utils.FileDataReaderWriters.Helpers;
using Curiosity.Utils.FileDataReaderWriters.Style;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;

namespace Curiosity.Utils.FileDataReaderWriters.Writers;

/// <summary>
/// Класс для записи данных в xlsx / xls формате для NPOI с разбитием на несколько файлов
/// </summary>
public class NpoiXlsMultiFileWriter : IFileWriter
{
    private readonly ILogger _logger;
    private          IReadOnlyList<CellData>? _headers;
    private          int _currentRow = 0;
    private          int _currentCell = 0;
    private          int _partNumber = 1;

    private readonly SXSSFWorkbook _workbook;
    private readonly ICellStyle _dataStyle;
    private          ISheet _sheet;
    private          FileStream _fileStream;
    
    private          Dictionary<int, ICellStyle> _cellStyles = new();
    
    private readonly string _savePath;
    private readonly string _fileName;

    /// <summary>
    /// Конструктор для записи множества эксель файлов через NPOI
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fileName">Название файла</param>
    /// <param name="savePath">Директория сохранения</param>
    /// <param name="rowAccessWindowSize">Количество строк, находящихся в памяти писателя</param>
    public NpoiXlsMultiFileWriter(string savePath, string fileName, ILogger logger, int rowAccessWindowSize = 100)
    {
        if (String.IsNullOrWhiteSpace(savePath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(savePath));
        if (String.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

        _savePath = savePath;
        _logger = logger;
        _fileName = fileName;
        _workbook = new SXSSFWorkbook(rowAccessWindowSize);
        _workbook.UseZip64 = UseZip64.On;
        _sheet = _workbook.CreateSheet();

        //настройка формата даты для ячеек
        var format = _workbook.CreateDataFormat();
        _dataStyle = _workbook.CreateCellStyle();
        _dataStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");
    }
    
    public int AddFormat(FormatSettings formatSettings)
    {
        var font = _workbook.CreateFont();
        font.IsBold = formatSettings.FontStyle.HasFlag(FontStyle.Bold);
        font.IsItalic = formatSettings.FontStyle.HasFlag(FontStyle.Italic);
        font.FontHeightInPoints = formatSettings.FontSize;
        
        var style = _workbook.CreateCellStyle();
        style.SetFont(font);

        style.Alignment = formatSettings.TextAlignment switch
        {
            TextAlignment.Left => HorizontalAlignment.Left,
            TextAlignment.Center => HorizontalAlignment.Center,
            TextAlignment.Right => HorizontalAlignment.Right,
            TextAlignment.Justify => HorizontalAlignment.Justify,
            _ => throw new ArgumentOutOfRangeException(nameof(formatSettings.TextAlignment))
        };

        style.WrapText = formatSettings.WrapText;

        var format = _workbook.CreateDataFormat();
        style.DataFormat = format.GetFormat(formatSettings.DataFormat);

        var formatNumber = _cellStyles.Count;
        _cellStyles.Add(formatNumber, style);

        return formatNumber;
    }

    public int AddDefaultFormat()
    {
        var formatNumber = _cellStyles.Count;
        _cellStyles.Add(formatNumber, _workbook.CreateCellStyle());

        return formatNumber;
    }
    
    public void AddHeaders(IReadOnlyList<CellData> data)
    {
        _headers = data ?? throw new ArgumentNullException(nameof(data));

        for (var i = 0; i < data.Count; i++)
        {
            var datum = data[i];
            Append(datum.Value, datum.Format);
        }
        
        EndLine();
    }

    public void AppendLine(IReadOnlyList<CellData> data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        for (var i = 0; i < data.Count; i++)
        {
            var datum = data[i];
            Append(datum.Value, datum.Format);
        }

        EndLine();
    }

    public void Append(object? value, int? format = null)
    {
        if (value is null || (value is string str && String.IsNullOrWhiteSpace(str)))
        {
            _currentCell++;
            return;
        }
        
        var row = _sheet.GetRow(_currentRow) ?? _sheet.CreateRow(_currentRow);
        var cell = row.CreateCell(_currentCell);

        switch (value)
        {
            case byte val:
                cell.SetCellValue(val);
                break;
            case short val:
                cell.SetCellValue(val);
                break;
            case int val:
                cell.SetCellValue(val);
                break;
            case long val:
                cell.SetCellValue(val);
                break;
            case float val:
                cell.SetCellValue(val);
                break;
            case double val:
                cell.SetCellValue(val);
                break;
            case decimal val:
                cell.SetCellValue(Convert.ToDouble(val));
                break;
            case bool val:
                cell.SetCellValue(val);
                break;
            case IRichTextString val:
                cell.SetCellValue(val);
                break;
            case string val:
                cell.SetCellValue(val);
                break;
            case DateOnly val:
                cell.SetCellValue(val);
                cell.CellStyle = _dataStyle;
                break;
            case DateTime val:
                cell.SetCellValue(val);
                cell.CellStyle = _dataStyle;
                break;
            default:
                _logger.LogWarning("Используем обычный ToString() (Type={Type}, Value={Value})", value.GetType(), value);
                cell.SetCellValue(value.ToString());
                break;
        }
        
        if (format is not null)
            cell.CellStyle = _cellStyles[(int)format];
        
        _currentCell++;
    }

    public void EndLine()
    {
        _currentRow++;
        _currentCell = 0;

        // если лимит - сбросим данные в файл
        if (_currentRow >= ExcelConstants.XlsxRowsMax)
            Flush();
    }

    public void Flush()
    {
        // если ничего нет
        // или только хедеры и это 2ой файл - выходим
        // (первый пустой файл с хедерами - сохраняем)
        if (_currentRow == 0 || 
            (_headers != null && _currentRow == 1 && _partNumber > 1))
            return;

        // сгенерим имя
        var fileName = _fileName;
        if (_partNumber > 1)
        {
            fileName =
                $"{Path.GetFileNameWithoutExtension(_fileName)}_part_{_partNumber}{Path.GetExtension(_fileName)}";
        }

        var outputFilePath = Path.Combine(_savePath, fileName);

        // сохраним
        _logger.LogDebug($"Сохраняем файл \"{outputFilePath}\"...");
        _fileStream = File.Open(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        _workbook.Write(_fileStream);
        _fileStream.Close();
        _logger.LogInformation($"Файл \"{outputFilePath}\" успешно сохранён");

        // обнулим состояние
        _workbook.RemoveSheetAt(0);
        _sheet = _workbook.CreateSheet();
        _partNumber++;
        _currentRow = 0;
        if (_headers != null)
            AddHeaders(_headers);
    }

    public void Dispose()
    {
        if (_fileStream is not null)
            Flush();
    }
}