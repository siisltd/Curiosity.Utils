namespace SIISLtd.Utils.FileDataReaderWriters.Readers;

/// <summary>
/// Общий интерфейс для классов, которые читает загруженные файлы построчно.
/// </summary>
/// <remarks>
/// Реализации должны внутри себя инкапсулировать все работу по чтению файла, обработке пустых строк. 
/// </remarks>
public interface IFileDataReader : IDisposable
{
    /// <summary>
    /// Максимальное количество строк для данного типа файла.
    /// </summary>
    /// <remarks>
    /// Нужно для того, чтобы не пытаться бесконечно читать данные из файла.
    /// </remarks>
    int MaxSupportedRowsCount { get; }

    /// <summary>
    /// Номер текущей строки, которая прочитана.
    /// </summary>
    int CurrentRowIdx { get; }

    /// <summary>
    /// Читает очередную строку в память и обрабатывает ее. 
    /// </summary>
    /// <returns>Была ли прочитана еще одна строка?</returns>
    /// <remarks>
    /// Данные строки можно получить через <see cref="GetRow"/>.
    /// </remarks>
    bool Read();

    /// <summary>
    /// Возвращает текущую прочитанную строку в виде отдельных ячеек.
    /// </summary>
    IReadOnlyList<string?>? GetRow();
}
