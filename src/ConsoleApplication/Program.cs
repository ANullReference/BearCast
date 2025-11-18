using ConsoleApplication.Abstractions;
using ConsoleApplication.OptionCommandLineHandler;
using Core;
using Core.Abstraction;
using Core.Domain;
using Infrastructure;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.Globalization;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        string language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        //todo: check in app settings if language is supported. 
        if (string.IsNullOrEmpty(language))
        {
            language = "en";
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("application_language.en.json", optional: true, reloadOnChange: true)
            .Build();

        // Create service collection and register services
        var services = new ServiceCollection();

        string appSettingsKey = "AppSettings";
        string applicationLanguageKey = "ApplicationLanguage";

        services.Configure<AppSettings>(config.GetSection(appSettingsKey));
        services.Configure<ApplicationLanguage>(config.GetSection(applicationLanguageKey));

        services.AddSingleton<ILogger, MyLogger>();
        services.AddSingleton<IMediaPlayerWrapper, MediaPlayerWrapper>();
        services.AddSingleton((libVlc) => new LibVLC(enableDebugLogs: config.GetValue<bool>("AppSettings:EnableDebugLogs")));
        services.AddSingleton<IFileManager, FileManager>();
        services.AddTransient<IPlayRequestManager, PlayManager>();
        services.AddTransient<IPlaylistRequestManager, PlaylistRequestManager>();
        services.AddTransient<ICommandLineHandler, CommandLineHandler>();

        services.AddHttpClient(Constants.HttpClientUrl);
        services.AddHttpClient(Constants.TvHttpClientUrl);

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        ICommandLineHandler? commandLineHandler = serviceProvider.GetService<ICommandLineHandler>();
        CancellationToken cancellationToken = new();

        if (commandLineHandler != null)
        {
            string command = string.Empty;
            RootCommand rootCommand = await commandLineHandler.CreateRootCommand();

            do
            {
                ParseResult parseResult = rootCommand.Parse(args);
                Environment.ExitCode = await parseResult.InvokeAsync(null, cancellationToken);
                Console.Write("Command: ");
                command = Console.ReadLine() ?? string.Empty;
                args = command.Split(" ");
            }
            while (!string.IsNullOrEmpty(command));
        }

        return Environment.ExitCode;
    }
}