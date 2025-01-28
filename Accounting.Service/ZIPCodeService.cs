using Accounting.Business;
using Accounting.Database;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Accounting.Service
{
  public class ZipCodeService : BaseService
  {
    public ZipCodeService() : base()
    {
        
    }

    public ZipCodeService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<List<ZipCode>> GetAllAsync(bool locationIsNull)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
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

      var zipCodesWithoutLocationGeography = await GetAllAsync(true);

      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
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
