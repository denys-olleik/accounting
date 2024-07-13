namespace Accounting.Common
{
    public static class FileInfoExtensions
    {
        public static string MoveToDirectory(this FileInfo file, string destinationDirectory)
        {
            string destinationFilePath = Path.Combine(destinationDirectory, file.Name);
            file.MoveTo(destinationFilePath);

            return destinationFilePath;
        }
    }
}