namespace Core.Abstraction;

public interface IParsePlaylist
{
    Task<List<Uri>> Parsem3u8SegmentUrls(string url, string baseUri);
}
