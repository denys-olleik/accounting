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

    public async Task<BoardState> GetFullBoardState(string requestingCountry)
    {
      FactoryManager factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var players = await factoryManager.GetPlayerManager().GetActivePlayersAsync();

      var cells = players
        .GroupBy(p => new { p.CurrentX, p.CurrentY })
        .Select(g =>
        {
          int friendlyCount = g.Count(p => p.Country == requestingCountry);
          bool hasEnemy = g.Any(p => p.Country != requestingCountry);
          string country = friendlyCount > 0 ? requestingCountry : null;

          return new BoardCell
          {
            X = g.Key.CurrentX,
            Y = g.Key.CurrentY,
            Country = country,
            FriendlyPlayerCount = friendlyCount,
            OccupiedByEnemy = hasEnemy
          };
        })
        .ToList();

      return new BoardState { State = cells };
    }

    public async Task<Player> RequestPosition(Guid guid, string ipAddress)
    {
      FactoryManager factoryManager = new FactoryManager(_databaseName, _databasePassword);

      // Fetch the player by GUID
      Player player = factoryManager.GetPlayerManager().GetParticipatingPlayerAsync(guid);

      // If player does not exist and no position is supplied, create/spawn new player
      if (player == null)
      {
        player = await factoryManager.GetPlayerManager()
            .CreateWithinBoundingBoxOfExistingPlayers(guid, player.Country, player.IpAddress);
      }

      return player;
    }
  }
}