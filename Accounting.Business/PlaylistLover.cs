using Accounting.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Business
{
  public class PlaylistLover : IIdentifiable<int>
  {
    public int PlaylistLoverID { get; set; }
    public string? Email { get; set; }
    public string? Code { get; set; }
    public DateTime CodeExpiration { get; set; }
    public bool Gender { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => throw new NotImplementedException();
  }
}