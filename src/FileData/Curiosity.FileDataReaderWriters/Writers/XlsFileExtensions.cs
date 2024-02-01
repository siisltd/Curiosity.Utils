using FlexCel.Core;
using FlexCel.XlsAdapter;

namespace Curiosity.FileDataReaderWriters.Writers;

public static class XlsFileExtensions
{
    public static (int HeaderXf, int DataXf) AddDefaultFormats(this XlsFile xls)
    {
        var fmt = xls.GetDefaultFormat;
        fmt.Font.Style = TFlxFontStyles.Bold;
        fmt.WrapText = false;
        var headerXf = xls.AddFormat(fmt);

        fmt = xls.GetDefaultFormat;
        var dataXf = xls.AddFormat(fmt);

        return (headerXf, dataXf);
    }
}