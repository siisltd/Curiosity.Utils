using FlexCel.Core;
using FlexCel.XlsAdapter;
using Microsoft.Extensions.Logging;
using SIISLtd.SSNG.Common;
using SIISLtd.Utils.FileDataReaderWriters.Style;

namespace SIISLtd.Utils.FileDataReaderWriters
{
    /// <summary>
    /// Класс для записи звонков в xlsx. Основная фишка - если количество строк больше лимита, то создаются отдельные файлы автоматчиески
    /// </summary>
    public class FlexCelXlsMultiFileWriter : IFileWriter
    {
        private readonly XlsFile _xls;
        private readonly ILogger _logger;
        
        private IReadOnlyList<CellData>? _headers;
        
        private readonly string       _savePath;
        private readonly string       _fileName;
        private readonly TFileFormats _fileFormat;
        private readonly bool         _isNeedToFitHeader;

        private          int _currentRow  = 1;
        private          int _currentCell = 1;
        private          int _partNumber  = 1;
        private readonly int _defaultHeaderXf;
        private readonly int _defaultDataXf;
        
        /// <param name="fileName">Имя файла с расширением</param>
        /// <param name="isNeedToFitHeader">если true, то заголовки растягиватся по ширине текста</param>
        public FlexCelXlsMultiFileWriter(
            string savePath,
            string fileName,
            TFileFormats fileFormat,
            bool isNeedToFitHeader,
            ILogger logger)
        {
            if (String.IsNullOrWhiteSpace(savePath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(savePath));
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

            _savePath = savePath;
            _fileName = fileName;
            _fileFormat = fileFormat;
            _isNeedToFitHeader = isNeedToFitHeader;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
            
            for (var i = 0; i < _headers.Count; i++)
            {
                var header = _headers[i]; 
                _xls.SetCellValue(_currentRow, _currentCell++, header.Value, header.Format ?? _defaultHeaderXf);
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
            // если новый файл - запишем заголовок
            if (_currentRow == 1 && _headers?.Count > 0)
            {
                for (var i = 0; i < _headers.Count; i++)
                {
                    var header = _headers[i]; 
                    _xls.SetCellValue(_currentRow, _currentCell++, header.Value, header.Format ?? _defaultHeaderXf);
                }
            
                EndLine();
            }
            
            _xls.SetCellValue(_currentRow, _currentCell++, value, format ?? _defaultDataXf);
        }
        
        public void EndLine()
        {
            _currentRow++;
            _currentCell = 1;
            
            // если лимит - сбросим данные в файл
            if (_currentRow >= ExcelConstants.XlsxRowsMax)
                Flush();
        }

        public void Flush()
        {
            // если ни чего нет - выходим
            if (_currentRow == 1)
                return;
            
            // подгоним ширину
            if (_isNeedToFitHeader && _headers?.Count > 0)
                _xls.AutofitCol(1, _headers.Count, false, 1.1f);

            // сгенерим имя
            var fileName = _fileName;
            if (_partNumber > 1)
            {
                fileName = $"{Path.GetFileNameWithoutExtension(_fileName)}_part_{_partNumber}{Path.GetExtension(_fileName)}";
            }
            var outputFilePath = Path.Combine(_savePath, fileName);
            
            // сохраним
            _logger.LogDebug($"Сохраняем файл \"{outputFilePath}\"...");
            _xls.Save(outputFilePath, _fileFormat);
            _logger.LogInformation($"Файл \"{outputFilePath}\" успешно сохранён");

            // обнулим состояние
            _xls.ClearSheet();
            _partNumber++;
            _currentRow = 1;
        }
        
        public void Dispose()
        {
            // нечего освобождать
        }
    }
}