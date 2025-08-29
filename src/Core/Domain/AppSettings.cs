namespace Core.Domain;

/// <summary>
/// Application settings
/// </summary>
public class AppSettings
{
    public BareCastLogLevel LogLevel { get; set; }

    public bool EnableDebugLogs { get; set; }

    public bool EnableDebugSerilog { get; set; }
}
