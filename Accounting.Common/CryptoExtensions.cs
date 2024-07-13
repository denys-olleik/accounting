using System.Security.Cryptography;

namespace Accounting.Common
{
  public static class GuidExtensions
  {
    public static Guid CreateSecureGuid()
    {
      using (var provider = new RNGCryptoServiceProvider())
      {
        var bytes = new byte[16];
        provider.GetBytes(bytes);
        return new Guid(bytes);
      }
    }
  }
}