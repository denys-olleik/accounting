using System.Security.Cryptography;

namespace Accounting.Common
{
  public class RandomHelper
  {

    public static string GenerateSecureAlphanumericString(int length, bool requireLetter = false)
    {
      if (length <= 0)
      {
        throw new ArgumentException("Length must be a positive integer.", nameof(length));
      }

      const string allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789";
      const string letters = "abcdefghijklmnopqrstuvwxyz";
      char[] chars = new char[length];

      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        byte[] randomBytes = new byte[length];

        // If a letter is required, place a random letter at the first position 
        if (requireLetter)
        {
          rng.GetBytes(randomBytes, 0, 1);
          chars[0] = letters[randomBytes[0] % letters.Length];
        }

        rng.GetBytes(randomBytes);

        // Fill the rest of the array with random alphanumeric characters
        for (int i = requireLetter ? 1 : 0; i < length; i++)
        {
          chars[i] = allowedChars[randomBytes[i] % allowedChars.Length];
        }

        // Optional: Shuffle the array to randomize the position of the guaranteed letter
        if (requireLetter)
        {
          ShuffleArray(chars, rng);
        }
      }

      return new string(chars);
    }

    private static void ShuffleArray(char[] array, RandomNumberGenerator rng)
    {
      byte[] randomBytes = new byte[1];
      for (int i = array.Length - 1; i > 0; i--)
      {
        rng.GetBytes(randomBytes);
        int j = randomBytes[0] % (i + 1);
        (array[i], array[j]) = (array[j], array[i]);
      }
    }
  }
}