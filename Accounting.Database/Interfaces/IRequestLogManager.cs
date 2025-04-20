using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IRequestLogManager : IGenericRepository<RequestLog, int>
  {
    Task<RequestLog> GetByIdAsync(int requestLogId);
    Task<int> UpdateResponseAsync(int requestLogID, string statusCode, long responseLength);
  }
}