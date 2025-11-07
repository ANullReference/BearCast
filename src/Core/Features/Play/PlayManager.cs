using Core.Abstraction;
using Core.Domain;

namespace Core;

/// <summary>
/// Handles requests the system does to the iptv
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="logger"></param>
/// <param name="mediaPlayerWrapper"></param>
/// <param name="fileManager"></param>
public class PlayManager(ILogger logger, IMediaPlayerWrapper mediaPlayerWrapper, IFileManager fileManager) : IPlayRequestManager
{
    private ILogger _log = logger;
    private IMediaPlayerWrapper _mediaPlayerWrapper = mediaPlayerWrapper;
    private IFileManager _fileManager = fileManager;

    public Task<ResponseObject<int>> Pause(Channel channel)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseObject<int>> Pause(Channel channel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseObject<int>> Play(string pathToFile, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathToFile, nameof(pathToFile));
        bool doesFileExist = _fileManager.Exists(pathToFile);

        ResponseObject<int> responseObject;

        if (!doesFileExist)
        {
            _log.Error("File {pathToFile} was not found.", pathToFile);
            responseObject = new(ResponseEnum.Fail, -1);

            return await Task.FromResult(responseObject);
        }

        await _mediaPlayerWrapper.Play(pathToFile, cancellationToken);

        responseObject = new(ResponseEnum.Success, 1);
        return responseObject;
    }

    public async Task<ResponseObject<int>> Play(Channel channel, CancellationToken cancellationToken)
    { 
        ArgumentNullException.ThrowIfNull(channel, nameof(channel));
        ArgumentException.ThrowIfNullOrEmpty(channel.Url, nameof(channel.Url));
        ArgumentException.ThrowIfNullOrWhiteSpace(channel.Url, nameof(channel.Url));

        ResponseObject<int> responseObject;

        await _mediaPlayerWrapper.Play(channel, cancellationToken);

        responseObject = new(ResponseEnum.Success, 1);
        return responseObject;
    }

    public Task<ResponseObject<int>> Record(Channel channel)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseObject<int>> Record(Channel channel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}