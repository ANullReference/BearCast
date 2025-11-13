using Core.Abstraction;
using Core.Domain;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Options;
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
    CancellationTokenSource? _cancellationTokenSource;// todo: dependency inject this into a wrapper and imitate the cancel behavior

    //private Task? _playVideo;

    private static readonly int MICRO_SLEEP = 1000;


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

        //_cancellationTokenSource = cancellationTokenSource;// todo: dependency inject this into a wrapper and imitate the cancel behavior
        LibVLCSharp.Shared.Core.Initialize();
    }


    private async Task<MediaPlayerStatusEnum> PlayVideo(Channel channel)
    {
        ArgumentException.ThrowIfNullOrEmpty(channel.Url, nameof(channel.Url));

        using Media media = new(_libVlc, channel.Url, FromType.FromLocation);
        using MediaPlayer mediaplayer = new(media);

        if (_cancellationTokenSource is null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        try
        {
            // Create a TaskCompletionSource to signal when the video finishes
            TaskCompletionSource<bool> videoFinishedSource = new();

            // Subscribe to the EndReached event
            mediaplayer.EndReached += (sender, e) =>
            {
                _log.Verbose("Video name: {name} link:{url} ended", channel.Name, channel.Url);
                mediaplayer.Stop();
                videoFinishedSource.SetResult(true);
            };

            mediaplayer.Play();

            await Task.Delay(MICRO_SLEEP, _cancellationTokenSource.Token);

            // Keep alive loop — allows cancellation
            while (mediaplayer.IsPlaying)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                await Task.Delay(MICRO_SLEEP, _cancellationTokenSource.Token); // cancellable delay
            }

            bool b = await videoFinishedSource.Task;

            mediaplayer.Stop();
            return MediaPlayerStatusEnum.Stopped;
        }
        catch (OperationCanceledException)
        {
            _log.Information("Playback of channel {channelName} with url {channelUrl} was cancelled.",
                channel.Name, channel.Url);

            media.Dispose();
            mediaplayer.Dispose();
        }
        catch (Exception ex)
        {
            _log.Error("An error occurred while trying to play channel {channelName} with url {channelUrl}. Exception: {exceptionMessage}",
                channel.Name, channel.Url, ex.Message);
        }

        return MediaPlayerStatusEnum.Playing;
    }

    public async Task<MediaPlayerStatusEnum> Play(string pathToFile)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathToFile, nameof(pathToFile));

        Channel channel = new()
        {
            Name = "LocalFile",
            Url = pathToFile
        };

        return await Play(channel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    public async Task<MediaPlayerStatusEnum> Play(Channel channel)
    {
        ArgumentNullException.ThrowIfNull(channel, nameof(channel));
        ArgumentException.ThrowIfNullOrEmpty(channel.Url, nameof(channel.Url));

        MediaPlayerStatusEnum mediaPlayerStatusEnum = MediaPlayerStatusEnum.Playing;

        if (_cancellationTokenSource != null)
        {
            await _cancellationTokenSource.CancelAsync();
        }

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            Task playVideo = Task.Factory.StartNew(async () => { await PlayVideo(channel); }
               , _cancellationTokenSource.Token);


            if (playVideo.IsCompletedSuccessfully || playVideo.IsCanceled)
            {
                mediaPlayerStatusEnum = MediaPlayerStatusEnum.Stopped;
            }
        }
        catch (Exception)
        {
            _log.Information("Playback of channel {channelName} with url {channelUrl} was cancelled.",
                channel.Name, channel.Url);
        }

        return await Task.FromResult(mediaPlayerStatusEnum);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<MediaPlayerStatusEnum> Stop()
    {
        if (_cancellationTokenSource != null)
        {
            await _cancellationTokenSource.CancelAsync();
        }

        return MediaPlayerStatusEnum.Stopped;
    }
}