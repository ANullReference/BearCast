
using Core;
using Core.Abstraction;
using Infrastructure;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Core.Domain;
using LibVLCSharp.Shared;
using System.Globalization;
using Core.Features.Playlist;


class Program
{
    public async Task Main(string[] args)
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
        services.AddTransient<IPlayManager, PlayManager>();
        services.AddTransient<IPlaylistManager, PlaylistManager>();
        services.AddTransient<IRequestManager, RequestManager>();

        string baseUri = config.GetValue<string>("AppSettings:BaseUri") ?? string.Empty;

        services.AddHttpClient(Constants.HttpClientUrl, (httpClient) =>
        {
            httpClient.BaseAddress = new Uri(baseUri);
        });

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        IPlayManager? serviceManager = serviceProvider.GetService<IPlayManager>();
        IRequestManager? manager = serviceProvider.GetService<IRequestManager>();


        if (manager != null && serviceManager != null)
        {
            Playlist response = await manager.GetPlaylist(@"x36xhzz.m3u8");
            ResponseObject<int> responseObjec = await serviceManager.PlayM3u8(baseUri + response.Channels.First().Url);
        }
    }
}