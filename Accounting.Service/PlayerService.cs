using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class PlayerService : BaseService
  {
    public PlayerService() : base()
    {
    }

    public PlayerService(string databaseName, string databasePassword)
      : base(databaseName, databasePassword)
    {

    }

    public async Task<BoardState> GetFullBoardState()
    {
      throw new NotImplementedException();
    }

    public async Task<Player> RequestPosition(int? x, int? y, Guid guid)
    {
      FactoryManager factoryManager = new FactoryManager(_databaseName, _databasePassword);

      // Fetch the player by GUID
      Player player = factoryManager.GetPlayerManager().GetParticipatingPlayerAsync(guid);

      // If player does not exist and no position is supplied, create/spawn new player
      if (player == null && (!x.HasValue || !y.HasValue))
      {
        player = await factoryManager.GetPlayerManager()
            .CreateWithinBoundingBoxOfExistingPlayers(guid, player.Country, player.IpAddress);
      }

      return player;
    }
  }
}