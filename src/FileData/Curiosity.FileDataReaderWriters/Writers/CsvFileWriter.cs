using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Curiosity.FileDataReaderWriters.Style;
using Curiosity.FileDataReaderWriters.Writers;
using global::System;
using global::System.Collections.Generic;
using global::System.IO;

namespace Curiosity.FileDataReaderWriters;

/// <summary>
/// Запись данных в .csv формате
/// </summary>
/// <remarks>
/// Расходует мало памяти, потому что запись идёт через потоки с буферизацией
/// </remarks>
public class CsvFileWriter : IFileWriter
{
    private readonly StreamWriter _streamWriter;
    private readonly CsvWriter    _csvWriter;

    public CsvFileWriter(string outputFilePath)
    {
        var configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            Delimiter = ";",
            Mode = CsvMode.RFC4180,
            Encoding = Encoding.UTF8,
        };

        _streamWriter = new StreamWriter(outputFilePath);
        _csvWriter = new CsvWriter(_streamWriter, configuration);
    }

    public int AddFormat(FormatSettings formatSettings) => 0;
    
    public int AddDefaultFormat() => 0;

    public void AddHeaders(IReadOnlyList<CellData> data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        
        AppendLine(data);
    }

    public void AppendLine(IReadOnlyList<CellData> data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        
        for (var i = 0; i < data.Count; i++)
        {
            Append(data[i].Value);
        }

        EndLine();
    }

    public void Append(object? value, int? format = null)
    {
        _csvWriter.WriteField(value);
    }

    public void EndLine()
    {
        // библиотека сама флашит строки периодически
        _csvWriter.NextRecord();
    }

    public void Flush()
    {
        _csvWriter.Flush();
        _streamWriter.Flush();
    }

    public void Dispose()
    {
        _csvWriter.Dispose();
        _streamWriter.Dispose();
    }
}