using Accounting.Business;
using Accounting.Database;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Accounting.Service
{
  public class ZipCodeService
  {
    public async Task<List<ZipCode>> GetAllAsync(bool locationIsNull)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetZIPCodeManager().GetAllAsync(locationIsNull);
    }

    public async Task UpdateNewZIPCodeLocations()
    {
      var csvFilePath = "uszips.csv";

      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        PrepareHeaderForMatch = args => args.Header.ToLower(),
      };

      var insertStatements = new List<string>();

      using (var reader = new StreamReader(csvFilePath))
      using (var csv = new CsvReader(reader, config))
      {
        var records = csv.GetRecords<ZipCode>();

        foreach (var record in records)
        {
          var insertStatement = GenerateInsertStatement(record);
          insertStatements.Add(insertStatement);
        }
      }

      List<ZipCode> zipCodesWithoutLocationGeography = await GetAllAsync(true);

      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetZIPCodeManager().UpdateLocationAsync(zipCodesWithoutLocationGeography);
    }

    static string GenerateInsertStatement(ZipCode zipCode)
    {
      var latitude = zipCode.Latitude.ToString(CultureInfo.InvariantCulture) ?? "NULL";
      var longitude = zipCode.Longitude.ToString(CultureInfo.InvariantCulture) ?? "NULL";
      var cityValue = zipCode.City.Replace("'", "''");
      var stateValue = zipCode.State2.Replace("'", "''");

      return $"""
           INSERT INTO "ZipCode" ("Zip5", "Latitude", "Longitude", "City", "State2")
           VALUES ('{zipCode.Zip5}', {latitude}, {longitude}, '{cityValue}', '{stateValue}');
       """;
    }
  }
}