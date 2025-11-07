using Core.Abstraction;
using Core.Domain;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using Channel = Core.Domain.Channel;

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

    public async Task<MediaPlayerStatusEnum> Play(string pathToFile, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathToFile, nameof(pathToFile));

        Task task = Task.Run(async () =>
        {
            using Media media = new(_libVlc, pathToFile, FromType.FromLocation);
            using MediaPlayer mediaplayer = new(media);

            try
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
                Console.ReadKey();

                bool b = await videoFinishedSource.Task;

                mediaplayer.Stop();
            }
            catch (TaskCanceledException)
            {
                mediaplayer?.Stop();
                _log.Information("Playback of file {pathToFile} was cancelled.",pathToFile);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while trying to play file {pathToFile}. Exception: {exceptionMessage}",
                    pathToFile, ex.Message);
            }
        }, cancellationToken);


        if (task.IsCompletedSuccessfully || task.IsCanceled)
        {
            return MediaPlayerStatusEnum.Stopped;
        }

        return await Task.FromResult(MediaPlayerStatusEnum.Playing);


    }

    public async Task<MediaPlayerStatusEnum> Play(Channel channel, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(channel, nameof(channel));    
        ArgumentException.ThrowIfNullOrEmpty(channel.Url, nameof(channel.Url));

        Task task = Task.Run(async () =>
        {
            using Media media = new(_libVlc, channel.Url, FromType.FromLocation);
            using MediaPlayer mediaplayer = new(media);

            try
            {    
                // Create a TaskCompletionSource to signal when the video finishes
                TaskCompletionSource<bool> videoFinishedSource = new();

                // Subscribe to the EndReached event
                mediaplayer.EndReached += (sender, e) =>
                {
                    _log.Verbose("Video name: {name} link:{httpLink} ended", channel.NAME, channel.Url);
                    videoFinishedSource.SetResult(true);
                };

                mediaplayer.Play();

                bool b = await videoFinishedSource.Task;

                mediaplayer.Stop();
            }
            catch(TaskCanceledException)
            {
                mediaplayer?.Stop();
                _log.Information("Playback of channel {channelName} with url {channelUrl} was cancelled.",
                    channel.NAME, channel.Url);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while trying to play channel {channelName} with url {channelUrl}. Exception: {exceptionMessage}",
                    channel.NAME, channel.Url, ex.Message);
            }
        }, cancellationToken);


        if (task.IsCompletedSuccessfully || task.IsCanceled)
        {
            return MediaPlayerStatusEnum.Stopped;
        }

        return await Task.FromResult(MediaPlayerStatusEnum.Playing);
    }
}