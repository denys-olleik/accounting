
using Accounting.Business;

namespace Accounting.Service
{
  public class PlaylistSubmissionService : BaseService
  {
    public PlaylistSubmissionService() : base()
    {

    }

    public PlaylistSubmissionService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task AddTracksToSubmissionAsync(int playlistSubmissionID, List<int> list)
    {
      throw new NotImplementedException();
    }

    public async Task<PlaylistSubmission> CreateSubmissionAsync(int playlistLoverID)
    {
      throw new NotImplementedException();
    }
  }
}