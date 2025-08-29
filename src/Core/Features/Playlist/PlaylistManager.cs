using Core.Abstraction;

namespace Core.Features.Playlist;

public class PlaylistManager(IRequestManager requestManager) : IPlaylistManager
{
    private IRequestManager _requestManager = requestManager;

    public async Task<string> GetPlaylist(string url)
    {
        return await _requestManager.HttpRequest(url);
    }
}
