namespace Core.Abstraction;

public interface IPlaylistManager
{
    Task<string> GetPlaylist(string url);
}
