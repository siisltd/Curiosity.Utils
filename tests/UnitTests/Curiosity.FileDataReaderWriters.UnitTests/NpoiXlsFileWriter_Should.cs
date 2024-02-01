using Curiosity.FileDataReaderWriters.Style;
using Curiosity.FileDataReaderWriters.Writers;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Xunit;

namespace Curiosity.FileDataReaderWriters.UnitTests
{
    /// <summary>
    /// Тесты записи для NPOI
    /// </summary>
    public class NpoiXlsFileWriter_Should
    {
        [Fact]
        public void WriteWithHeaders()
        {
            //ARRANGE
            var fileName = "testFile.xlsx";
            var header1 = "Header1";
            var header2 = "Header2";
            var header3 = "Header3";
            var headerFormat = new FormatSettings()
            {
                FontSize = 20,
                FontStyle = FontStyle.Bold,
                TextAlignment = TextAlignment.Center,
                DataFormat = "@"
            };
            
            var logger = new Logger<NpoiXlsMultiFileWriter>(new LoggerFactory());
            var fileWriter = new NpoiXlsFileWriter(fileName, logger);
            var formatNumber = fileWriter.AddFormat(headerFormat);
            
            var headers = new List<CellData>();
            headers.Add(new CellData(header1, formatNumber));
            headers.Add(new CellData(header2));
            headers.Add(new CellData(header3, formatNumber));

            var firstRow = new List<CellData>();
            firstRow.Add(new CellData(1.23));
            firstRow.Add(new CellData(DateTime.Now));
            firstRow.Add(new CellData("blabla"));
            firstRow.Add(new CellData(null));
            
            var secondRow = new List<CellData>();
            secondRow.Add(new CellData(3.43));
            secondRow.Add(new CellData(DateTime.UtcNow));
            secondRow.Add(new CellData("blabla2"));
            
            //ACT
            fileWriter.AddHeaders(headers);
            fileWriter.AppendLine(firstRow);
            fileWriter.AppendLine(secondRow);
            fileWriter.Dispose();

            //ASSERT
            try
            {
                var stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                var fileReader = new XSSFWorkbook(stream);
                var sheet = fileReader.GetSheetAt(0);
            
                var headersResult = sheet.GetRow(0);
                Assert.Equal(headersResult.GetCell(0).StringCellValue, header1);
                Assert.Equal(headersResult.GetCell(1).StringCellValue, header2);
                Assert.Equal(headersResult.GetCell(2).StringCellValue, header3);
                Assert.Equal(headersResult.GetCell(0).CellStyle.Alignment, HorizontalAlignment.Center);
                Assert.Equal(headersResult.GetCell(2).CellStyle.GetDataFormatString(), headerFormat.DataFormat);
            
                var firstRowResult = sheet.GetRow(1);
                Assert.Equal(firstRowResult.GetCell(0).NumericCellValue, firstRow[0].Value);
                Assert.Equal(firstRowResult.GetCell(1).DateCellValue.ToLongDateString(), DateTime.Parse(firstRow[1].Value.ToString()).ToLongDateString());
                Assert.Equal(firstRowResult.GetCell(2).StringCellValue, firstRow[2].Value);
            
                var secondRowResult = sheet.GetRow(2);
                Assert.Equal(secondRowResult.GetCell(0).NumericCellValue, secondRow[0].Value);
                Assert.Equal(secondRowResult.GetCell(1).DateCellValue.ToLongDateString(), DateTime.Parse(secondRow[1].Value.ToString()).ToLongDateString());
                Assert.Equal(secondRowResult.GetCell(2).StringCellValue, secondRow[2].Value);
            }
            finally{
                File.Delete(fileName);
            }
        }
    }
}