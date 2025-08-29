using Core.Domain;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ILogger = Core.Abstraction.ILogger;

namespace Infrastructure;


/// <summary>
/// concrete logger wrapping around logger.
/// Please use a singleton to instantiate. 
/// </summary>
public class MyLogger : ILogger
{
    private AppSettings _appSettings;
    private Logger _log;

    public MyLogger(IOptions<AppSettings> options)
    {
        _appSettings = options.Value;
        LogEventLevel logEventLevel;

        logEventLevel = _appSettings.LogLevel switch
        {
            BareCastLogLevel.Verbose => LogEventLevel.Verbose,
            BareCastLogLevel.Debug => LogEventLevel.Debug,
            BareCastLogLevel.Information => LogEventLevel.Information,
            BareCastLogLevel.Warning => LogEventLevel.Information,
            BareCastLogLevel.Error => LogEventLevel.Error,
            BareCastLogLevel.Fatal => LogEventLevel.Fatal,
            _ => throw new NotSupportedException($"Log level {_appSettings.LogLevel} not supported"),
        };

        _log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt")
            .MinimumLevel.Is(logEventLevel)
            .CreateLogger();

        Serilog.Debugging.SelfLog.Enable(Console.Error);
    }

    public void Debug(string message, params object[] parameters)
    {
        _log.Debug(message, parameters);
    }

    public void Error(string message, Exception exception, params object[] parameters)
    {
        _log.Error(message, exception, parameters);
    }

    public void Error(string message, params object[] parameters)
    {
        _log.Error(message, parameters);
    }


    public void Fatal(Exception exception)
    {
        _log.Fatal(exception.Message);
    }

    public void Information(string message, params object[] parameters)
    {
        _log.Information(message, parameters);
    }

    public void Verbose(string message, params object[] parameters)
    {
        _log.Verbose(message, parameters);
    }

    public void Warning(string message, params object[] parameters)
    {
        _log.Warning(message, parameters);
    }
}
