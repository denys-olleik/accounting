//using Microsoft.Extensions.Logging;
//using System;

//public class DatabaseLogger : ILogger
//{
//  private readonly string _categoryName;

//  public DatabaseLogger(string categoryName)
//  {
//    _categoryName = categoryName;
//  }

//  public IDisposable BeginScope<TState>(TState state) => null!;

//  public bool IsEnabled(LogLevel logLevel)
//  {
//    // You can add more sophisticated filtering logic here
//    return true;
//  }

//  public void Log<TState>(LogLevel logLevel, EventId eventId,
//      TState state, Exception? exception, Func<TState, Exception?, string> formatter)
//  {
//    if (!IsEnabled(logLevel))
//      return;

//    var message = formatter(state, exception);

//    // TODO: Replace this with your actual database write logic
//    // Example: SaveLogToDatabase(_categoryName, logLevel, message, exception?.ToString(), eventId.Id);

//    Console.WriteLine($"[{DateTime.UtcNow}] {_categoryName} [{logLevel}]: {message} {exception}");
//  }

//  // Example stub for database write (implement as needed)
//  private void SaveLogToDatabase(string category, LogLevel level, string message, string? exception, int eventId)
//  {
//    // Implement your database logic here
//  }
//}