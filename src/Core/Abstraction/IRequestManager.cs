using Core.Domain;

namespace Core.Abstraction;

public interface IRequestManager
{
    Task<string> HttpRequest(string url);
    Task<Playlist> GetPlaylist(string url);
}
