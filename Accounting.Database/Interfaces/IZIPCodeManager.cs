using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IZIPCodeManager : IGenericRepository<ZIPCode, int>
  {
    Task<List<ZIPCode>> GetAllAsync(bool locationIsNull);
    Task<int> UpdateLocationAsync(string zip5, float latitude, float longitude);
  }
}