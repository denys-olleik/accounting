using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IPlayerManager : IGenericRepository<Player, int>
  {
    Task<Player> CreateWithinBoundingBoxOfExistingPlayers(Guid guid, string country, string ipAddress);
    Task<List<Player>> GetActivePlayersAsync();
    Task<Player?> GetParticipatingPlayerAsync(Guid guid);
  }
}