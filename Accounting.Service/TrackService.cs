
using Accounting.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Service
{
  public class TrackService : BaseService
  {
    public TrackService() : base()
    {

    }
    public TrackService(string databaseName, string databasePassword) 
      : base(databaseName, databasePassword)
    {

    }

    public async Task<List<Track>> UpsertRangeAsync(List<Track> tracks)
    {
      throw new NotImplementedException();
    }
  }
}