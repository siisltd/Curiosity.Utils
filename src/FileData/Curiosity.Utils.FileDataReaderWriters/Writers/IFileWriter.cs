using Curiosity.Utils.FileDataReaderWriters.Style;

namespace Curiosity.Utils.FileDataReaderWriters.Writers;

/// <summary>
/// Интерфейс для простого построчного вывода данных
/// </summary>
public interface IFileWriter : IDisposable
{
    /// <summary>
    /// Добавляет формат с указанными настройками
    /// </summary>
    /// <param name="formatSettings"></param>
    /// <returns></returns>
    int AddFormat(FormatSettings formatSettings);
    
    /// <summary>
    /// Добавляет формат по умолчанию. В Excel он называется "Общий/General"
    /// </summary>
    /// <returns></returns>
    int AddDefaultFormat();
    
    /// <summary>
    /// Добавляет заголовки таблицы 
    /// </summary>
    public void AddHeaders(IReadOnlyList<CellData> data);

    /// <summary>
    /// Добавляет массив данных в 1 строку.
    /// Автоматически переходит на новую строку
    /// </summary>
    public void AppendLine(IReadOnlyList<CellData> data);
    
    /// <summary>
    /// Добавляет данные в текущую строку без перехода на новую
    /// </summary>
    public void Append(object? value, int? format = null);
    
    /// <summary>
    /// Переход на новую строку
    /// </summary>
    public void EndLine();
    
    /// <summary>
    /// Записывает данные из буфера в файл
    /// </summary>
    public void Flush();
}