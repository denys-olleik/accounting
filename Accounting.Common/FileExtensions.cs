namespace Accounting.Common
{
  public static class FileExtensions
  {
    public async static Task<string> SaveFile(this File file, string path)
    {
      var filePath = Path.Combine(path, $"{RandomHelper.GenerateSecureAlphanumericString(10)}{Path.GetExtension(file.FileName)}");

      var directoryPath = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await using var stream = new FileStream(filePath, FileMode.Create);
      await file.Stream.CopyToAsync(stream);

      return filePath;
    }
  }
}