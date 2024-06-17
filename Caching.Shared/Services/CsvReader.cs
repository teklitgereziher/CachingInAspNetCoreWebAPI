using Caching.Shared.Models;
using Caching.Shared.Services.Interfaces;
using CsvHelper.Configuration;
using System.Globalization;
using System.Reflection;

namespace Caching.Shared.Services
{
  public class CsvReader : ICsvReader
  {
    public IEnumerable<BggRecord> Read()
    {
      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        HasHeaderRecord = true,
        Delimiter = ",",
      };

      string filePath = Path.Combine(
        Path.GetDirectoryName(
          Assembly.GetExecutingAssembly().Location),
        @"Data/bgg_dataset.csv");
      var reader = new StreamReader(filePath);
      var csv = new CsvHelper.CsvReader(reader, config);
      var records = csv.GetRecords<BggRecord>();

      return records;
    }
  }
}
