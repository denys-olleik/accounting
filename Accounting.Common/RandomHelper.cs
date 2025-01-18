using System.Security.Cryptography;

namespace Accounting.Common
{
  public class RandomHelper
  {
    public static string GenerateSecureAlphanumericString(int length)
    {
      const string allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789";
      char[] chars = new char[length];

      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        byte[] randomBytes = new byte[length];
        rng.GetBytes(randomBytes);

        for (int i = 0; i < length; i++)
        {
          chars[i] = allowedChars[randomBytes[i] % allowedChars.Length];
        }
      }

      return new string(chars);
    }
  }
}