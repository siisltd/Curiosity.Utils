using System.Text;
using Curiosity.FileDataReaderWriters.Style;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using global::System;
using global::System.Collections.Generic;

namespace Curiosity.FileDataReaderWriters.Writers;

/// <summary>
/// Запись данных в xlsx / xls формате
/// </summary>
public class FlexCelXlsFileWriter : IFileWriter
{
    private readonly XlsFile _xls;

    private IReadOnlyList<CellData>? _headers;

    private readonly string       _outputFilePath;
    private readonly TFileFormats _fileFormat;
    private readonly bool         _isNeedToFitHeader;
    
    private          int _currentRow  = 1;
    private          int _currentCell = 1;
    private readonly int _defaultHeaderXf;
    private readonly int _defaultDataXf;

    /// <param name="outputFilePath">Полное имя файла</param>
    /// <param name="isNeedToFitHeader">если true, то заголовки растягиваются по ширине текста</param>
    public FlexCelXlsFileWriter(
        string outputFilePath,
        TFileFormats fileFormat,
        bool isNeedToFitHeader)
    {
        _outputFilePath = outputFilePath;
        _fileFormat = fileFormat;
        _isNeedToFitHeader = isNeedToFitHeader;

        _xls = new XlsFile(1, true);
        (_defaultHeaderXf, _defaultDataXf) = _xls.AddDefaultFormats();
    }
    
    public int AddFormat(FormatSettings formatSettings)
    {
        var fmt = _xls.GetDefaultFormat;
        fmt.Format = formatSettings.DataFormat;
        fmt.Font.Size20 = formatSettings.FontSize * 20;
        fmt.WrapText = formatSettings.WrapText;

        fmt.HAlignment = formatSettings.TextAlignment switch
        {
            TextAlignment.Left => THFlxAlignment.left,
            TextAlignment.Center => THFlxAlignment.center,
            TextAlignment.Right => THFlxAlignment.right,
            TextAlignment.Justify => THFlxAlignment.justify,
            _ => throw new ArgumentOutOfRangeException(nameof(formatSettings.TextAlignment))
        };

        fmt.Font.Style = formatSettings.FontStyle switch
        {
            FontStyle.None => TFlxFontStyles.None,
            FontStyle.Bold => TFlxFontStyles.Bold,
            FontStyle.Italic => TFlxFontStyles.Italic,
            _ => throw new ArgumentOutOfRangeException(nameof(formatSettings.FontStyle))
        };

        return _xls.AddFormat(fmt);
    }
    
    public int AddDefaultFormat()
    {
        return _xls.AddFormat(_xls.GetDefaultFormat);
    }

    public void AddHeaders(IReadOnlyList<CellData> data)
    {
        _headers = data ?? throw new ArgumentNullException(nameof(data));
        
        for (var i = 0; i < data.Count; i++)
        {
            var datum = data[i];
            Append(datum.Value, datum.Format ?? _defaultHeaderXf);
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
        _xls.SetCellValue(_currentRow, _currentCell++, value, format ?? _defaultDataXf);
    }

    public void EndLine()
    {
        _currentRow++;
        _currentCell = 1;
    }

    public void Flush()
    {
        // подгоним ширину, если надо
        if (_isNeedToFitHeader && _headers?.Count > 0)
            _xls.AutofitCol(1, _headers.Count, false, 1.1f);
        
        _xls.Save(_outputFilePath, _fileFormat, ';', Encoding.UTF8);
    }
    
    public void Dispose()
    {
        // тут нечего освобождать
    }
}
