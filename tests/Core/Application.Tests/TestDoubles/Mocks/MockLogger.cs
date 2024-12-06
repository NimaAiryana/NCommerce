using Microsoft.Extensions.Logging;

namespace Application.Tests.TestDoubles.Mocks;

public class MockLogger<T> : ILogger<T>
{
    private readonly List<LogEntry> _loggedMessages = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _loggedMessages.Add(new LogEntry(logLevel, formatter(state, exception)));
    }

    public void VerifyLoggedMessage(string expectedMessage)
    {
        if (!_loggedMessages.Any(entry => entry.Message.Contains(expectedMessage)))
            throw new Exception($"Expected log message containing '{expectedMessage}' was not found");
    }

    public void VerifyLoggedMessageCount(int expectedCount)
    {
        if (_loggedMessages.Count != expectedCount)
            throw new Exception($"Expected {expectedCount} log messages but found {_loggedMessages.Count}");
    }

    public void VerifyLogLevel(string message, LogLevel expectedLevel)
    {
        var entry = _loggedMessages.FirstOrDefault(e => e.Message.Contains(message));
        if (entry == null)
            throw new Exception($"Message containing '{message}' was not found");

        if (entry.Level != expectedLevel)
            throw new Exception($"Expected log level {expectedLevel} but found {entry.Level}");
    }
    
    private record LogEntry(LogLevel Level, string Message);
}