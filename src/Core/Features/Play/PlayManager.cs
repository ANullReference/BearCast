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
public class PlayManager(ILogger logger, IMediaPlayerWrapper mediaPlayerWrapper, IFileManager fileManager) : IPlayManager
{
    private ILogger _log = logger;
    private IMediaPlayerWrapper _mediaPlayerWrapper = mediaPlayerWrapper;
    private IFileManager _fileManager = fileManager;

    public async Task<ResponseObject<int>> Play(string pathToFile)
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

        await _mediaPlayerWrapper.Play(pathToFile);

        responseObject = new(ResponseEnum.Success, 1);
        return await Task.FromResult(responseObject);
    }

    public async Task<ResponseObject<int>> PlayM3u8(string httpLink)
    { 
        ArgumentException.ThrowIfNullOrEmpty(httpLink, nameof(httpLink));
        
        ResponseObject<int> responseObject;

        await _mediaPlayerWrapper.PlayM3u8(httpLink);

        responseObject = new(ResponseEnum.Success, 1);
        return await Task.FromResult(responseObject);
    }
}