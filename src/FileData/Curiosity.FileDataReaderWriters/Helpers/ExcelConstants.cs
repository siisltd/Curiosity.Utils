namespace Curiosity.FileDataReaderWriters.Helpers;

public static class ExcelConstants
{
    /// <summary>
    /// Максимальное число строк в XLSX файле
    /// </summary>
    public const int XlsxRowsMax = 1048576;
        
    /// <summary>
    /// Максимальное число колонок в XLSX файле
    /// </summary>
    public const int XlsxColMax = 16384;

    /// <summary>
    /// Максимальное число строк в XLS файле
    /// </summary>
    public const int XlsRowsMax = 65536;
        
    /// <summary>
    /// Максимальное число колонок в XLS файле
    /// </summary>
    public const int XlsColMax = 256;

    /// <summary>
    /// Максимальное количество символов вмещающееся в ячейку
    /// </summary>
    public const int MaxCellLength = 32759;
}