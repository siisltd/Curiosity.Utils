namespace Curiosity.FileDataReaderWriters.Style;

/// <summary>
/// Настройки формата ячеек
/// </summary>
public class FormatSettings
{
    /// <summary>
    /// Начертание шрифта
    /// </summary>
    public FontStyle FontStyle { get; set; }
    
    /// <summary>
    /// Горизонтальное выранивание текста
    /// </summary>
    public TextAlignment TextAlignment { get; set; }
    
    /// <summary>
    /// Перенос слов на новую строку
    /// </summary>
    public bool WrapText { get; set; }
    
    /// <summary>
    /// Размер шрифта в пунктах
    /// </summary>
    public int FontSize { get; set; } = 10;
    
    /// <summary>
    /// Строка, указывающая формат (текст/число/деньги и т.д.). Можно посмотреть в форматах Excel
    /// </summary>
    public string DataFormat { get; set; } = "@";
}