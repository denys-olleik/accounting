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

    public async Task UpdateNewZIPCodeLocations()
    {
      List<ZIPCode> zipCodesWithoutLocationGeography = await GetAllAsync(true);

      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetZIPCodeManager().UpdateLocationAsync(zipCodesWithoutLocationGeography);
    }
  }
}