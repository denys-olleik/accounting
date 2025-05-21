using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IPlayerManager : IGenericRepository<Player, int>
  {
    Task<Player> CreateWithinBoundingBoxOfExistingPlayers(Guid guid, string country, string ipAddress);
    Player GetParticipatingPlayerAsync(Guid guid);
  }
}