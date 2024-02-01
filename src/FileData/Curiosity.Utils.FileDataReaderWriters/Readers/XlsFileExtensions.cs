using FlexCel.XlsAdapter;

namespace Curiosity.Utils.FileDataReaderWriters.Readers
{
    /// <summary>
    /// Методы расширения для <see cref="XlsFile"/> в TMS FlexCell
    /// </summary>
    internal static class XlsFileExtensions
    {
        /// <summary>
        /// Возвращает обрезанную строку из указанной ячейки или пустую строку, если в ячейке ничего нет
        /// </summary>
        public static string GetTrimmedStringFromCell(this XlsFile xls, int rIdx, int cIdx)
        {
            return (xls.GetStringFromCell(rIdx, cIdx) ?? String.Empty).Trim();
        }
    }
}
