namespace Core.Abstraction;

public interface ILogger
{
    void Debug(string message, params object[] parameters);
    void Verbose(string message, params object[] parameters);
    void Information(string message, params object[] parameters);
    void Warning(string message, params object[] parameters);
    void Error(string message, params object[] parameters);
    void Error(string message, Exception exception, params object[] parameters);
    void Fatal(Exception exception);
}
