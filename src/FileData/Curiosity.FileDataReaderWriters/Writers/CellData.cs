namespace Curiosity.FileDataReaderWriters.Writers;

/// <summary>
/// Данные ячейки с форматом
/// </summary>
public class CellData
{
    public object? Value { get; }
    public int? Format { get; }

    public CellData(object? value, int? format = null)
    {
        Value = value;
        Format = format;
    }
}