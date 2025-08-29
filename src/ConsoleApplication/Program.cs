
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

string language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

//todo: check in app settings is language is supported. 
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

services.AddHttpClient(Constants.HttpClientUrl);

// Build service provider
var serviceProvider = services.BuildServiceProvider();

//IPlayManager? serviceManager = serviceProvider.GetService<IPlayManager>();
IRequestManager? manager = serviceProvider.GetService<IRequestManager>();
//serviceManager?.Play(@"E:\Phone Back up\20240928_082005.mp4");
//serviceManager?.PlayM3u8(@"https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8");

if (manager != null)
{
    var response = await manager.GetPlaylist(@"https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8");
}


// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");