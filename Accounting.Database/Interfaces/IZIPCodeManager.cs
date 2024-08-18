using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IZIPCodeManager : IGenericRepository<ZipCode, int>
  {
    Task<List<ZipCode>> GetAllAsync(bool locationIsNull);
    Task<int> UpdateLocationAsync(List<ZipCode> zipCodes);
  }
}