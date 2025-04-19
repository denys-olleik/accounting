using Microsoft.Extensions.Logging;

public class DatabaseLoggerProvider : ILoggerProvider
{
  public ILogger CreateLogger(string categoryName)
  {
    return new DatabaseLogger(categoryName);
  }

  public void Dispose()
  {
    // Dispose resources if necessary
  }
}