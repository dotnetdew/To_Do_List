using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.Helpers;

public class CsvBaseService<T>
{
    private readonly CsvConfiguration _configuration;
    public CsvBaseService()
    {
        _configuration = GetConfiguration();
    }


    public byte[] UploadFile(IEnumerable data)
    {
        using (var stream = new MemoryStream())
        {
            using (var streamWriter =  new StreamWriter(stream))
            {
                using (var csvWriter = new CsvWriter(streamWriter, _configuration))
                {
                    csvWriter.WriteRecords(data);
                    streamWriter.Flush();
                    return stream.ToArray();
                }
            }
        }
    }

    public CsvConfiguration GetConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            Encoding = Encoding.UTF8,
            NewLine = "\r\n",
        };
    }
}
