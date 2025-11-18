using Core.Domain;

namespace Core.Abstraction;

/// <summary>
/// Rename this to ihttprequestmanager
/// </summary>
public interface IPlaylistRequestManager
{
    Task<string> HttpRequest(string url);
    Task<Playlist> GetPlaylist(string url);
    Task<Playlist> RefreshPlaylist(string url);
    Task<IEnumerable<Channel>> SearchPlaylist(string name);
    Task<List<Uri>> Parsem3u8SegmentUrls(string playlistText, string baseUri);
}
