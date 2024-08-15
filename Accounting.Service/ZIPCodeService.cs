//CREATE TABLE "ZipCode"
//(
//    "ID" SERIAL PRIMARY KEY NOT NULL,
//    "Zip5" VARCHAR(5) NOT NULL,
//    "Latitude" FLOAT NOT NULL,
//    "Longitude" FLOAT NOT NULL,
//    "City" VARCHAR(50) NOT NULL,
//    "State2" VARCHAR(2) NOT NULL,
//    "Location" GEOGRAPHY(Point, 4326) NULL
//);

using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ZIPCodeService
  {
    public async Task<List<ZIPCode>> GetAllAsync(bool locationIsNull)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetZIPCodeManager().GetAllAsync(locationIsNull);
    }

    public async Task UpdateLocationAsync()
    {
      List<ZIPCode> zipCodesWithoutLocationGeography = await GetAllAsync(true);

      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetZIPCodeManager().UpdateLocationAsync(zipCodesWithoutLocationGeography);
    }
  }
}