using Curiosity.FileDataReaderWriters.Style;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;

namespace Curiosity.FileDataReaderWriters.Writers;

/// <summary>
/// Класс для записи данных в xlsx / xls формате для NPOI
/// </summary>
public class NpoiXlsFileWriter : IFileWriter
{
    private          int _currentRow = 0;
    private          int _currentCell = 0;
    private readonly string _outputFilePath;
    private readonly ILogger _logger;

    private readonly SXSSFWorkbook _workbook;
    private readonly ISheet _sheet;
    private readonly ICellStyle _dataStyle;
    private          FileStream _fileStream;
    
    private          Dictionary<int, ICellStyle> _cellStyles = new();

    /// <summary>
    /// Конструктор для записи эксель файла через NPOI
    /// </summary>
    /// <param name="outputFilePath">Адрес итогового файла</param>
    /// <param name="rowAccessWindowSize">Количество строк, находящихся в памяти писателя</param>
    public NpoiXlsFileWriter(string outputFilePath, ILogger logger, int rowAccessWindowSize = 100)
    {
        _outputFilePath = outputFilePath;
        _logger = logger;
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
        if (data is null)
            throw new ArgumentNullException(nameof(data));

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
    }

    public void Flush()
    {
    }

    /// <summary>
    /// Записываем данные в файл и закрываем стрим.
    /// </summary>
    public void Dispose()
    {
        _fileStream = File.Open(_outputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _workbook.Write(_fileStream);
        _fileStream.Close();
    }
}