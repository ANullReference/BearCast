using Core.Abstraction;
using Core.Domain;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Options;

namespace Infrastructure;

/// <summary>
/// 
/// 
/// 
/// </summary> 
/// <example>
///     using var libvlc = new LibVLC(enableDebugLogs: true);
///     using var media = new Media(libvlc, new Uri(@"C:\tmp\big_buck_bunny.mp4"));
///     using var mediaplayer = new MediaPlayer(media);
///     mediaplayer.Play();
///     
///     Console.ReadKey();
/// </example>
public class MediaPlayerWrapper : IMediaPlayerWrapper
{
    private LibVLC _libVlc;
    private ILogger _log;
    private ApplicationLanguage _appLanguage;
    private IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Concrete implementation of video playing.
    /// </summary>
    /// <param name="libVLC"></param>
    public MediaPlayerWrapper(LibVLC libVLC, ILogger logger, IOptions<ApplicationLanguage> options, IHttpClientFactory httpClientFactory)
    {
        _libVlc = libVLC;
        _log = logger;
        _appLanguage = options.Value;
        _httpClientFactory = httpClientFactory;

        LibVLCSharp.Shared.Core.Initialize();
    }

    public async Task Play(string pathToFile)
    {
        using Media media = new(_libVlc, new Uri(pathToFile));
        using MediaPlayer mediaplayer = new(media);
        {
            // Create a TaskCompletionSource to signal when the video finishes
            TaskCompletionSource<bool> videoFinishedSource = new();

            // Subscribe to the EndReached event
            mediaplayer.EndReached += (sender, e) =>
            {
                _log.Verbose("Video {pathToFile} ended", pathToFile);
                videoFinishedSource.SetResult(true);
            };

            mediaplayer.Play();
            bool b = await videoFinishedSource.Task;

            Console.ReadLine();

            mediaplayer.Stop();
        }
    }

    public async Task PlayM3u8(string httpLink)
    {
        using Media media = new(_libVlc, httpLink, FromType.FromLocation);
        using MediaPlayer mediaplayer = new(media);
        {
            // Create a TaskCompletionSource to signal when the video finishes
            TaskCompletionSource<bool> videoFinishedSource = new();

            // Subscribe to the EndReached event
            mediaplayer.EndReached += (sender, e) =>
            {
                _log.Verbose("Video {httpLink} ended", httpLink);
                videoFinishedSource.SetResult(true);
            };

            mediaplayer.Play();
 
            Console.ReadLine();
            bool b = await videoFinishedSource.Task;

            mediaplayer.Stop();
        }
    }
}