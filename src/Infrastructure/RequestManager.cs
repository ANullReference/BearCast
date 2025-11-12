using Core;
using Core.Abstraction;
using Core.Domain;

namespace Infrastructure;

public class RequestManager : IRequestManager
{
    private IHttpClientFactory _httpClientFactory;
    private ILogger _logger;
    private Playlist? _playlist;// TODO: consider ICacheManager to handle playlist instance

    public RequestManager(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> HttpRequest(string url)
    {
        ArgumentException.ThrowIfNullOrEmpty(url);
        try
        {
            HttpClient client = _httpClientFactory.CreateClient(Constants.HttpClientUrl);
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("link {url} seems to not work. Status code: {statusCode} Response: {response}", url, response.StatusCode, response.Content);
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception exception)
        {
            _logger.Error("Method name: {methodName} Parameter: {url}", exception, nameof(HttpRequest), url);
        }

        return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// 
    /// Sample response:
    /// 
    /// #EXTM3U
    /// #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2149280,CODECS=\"mp4a.40.2,avc1.64001f\",RESOLUTION=1280x720,NAME=\"720\"
    /// url_0/193039199_mp4_h264_aac_hd_7.m3u8
    /// #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=246440,CODECS=\"mp4a.40.5,avc1.42000d\",RESOLUTION=320x184,NAME=\"240\"
    /// url_2/193039199_mp4_h264_aac_ld_7.m3u8
    /// #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=460560,CODECS=\"mp4a.40.5,avc1.420016\",RESOLUTION=512x288,NAME=\"380\"
    /// url_4/193039199_mp4_h264_aac_7.m3u8
    /// #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=836280,CODECS=\"mp4a.40.2,avc1.64001f\",RESOLUTION=848x480,NAME=\"480\"
    /// url_6/193039199_mp4_h264_aac_hq_7.m3u8
    /// #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"
    /// url_8/193039199_mp4_h264_aac_fhd_7.m3u8
    /// 
    /// </example>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<Playlist> GetPlaylist(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            throw new Exception($"{url} not well formed");
        }

        _logger.Debug($"Getting playlist in method {nameof(GetPlaylist)}");

        if (_playlist != null)
        {
            return _playlist;
        }

        string playlistString = await HttpRequest(url);
        playlistString = playlistString.Trim();

        //_playlist = new Playlist();

        using (StringReader reader = new(playlistString))
        {
            string firstLine = reader.ReadLine() ?? string.Empty;

            Playlist playlist = new Playlist() { ExtM3U = firstLine };

            string line, nextLine;

            while (reader.Peek() != -1)
            {
                line = reader.ReadLine() ?? string.Empty;
                nextLine = reader.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(nextLine))
                {
                    _logger.Warning("Empty entry on this line: line 1 {line1} and line 2 {line2}", line, nextLine);
                    continue;
                }

                line = line.RemoveComasWithinDoubleQuotes();
                //string[] data = line.Split(",");

                string[] data = line.Split(" ");

                Channel channel = new();

                foreach (string d in data.Where(w => w.Contains(':') || w.Contains('=')))
                {
                    string[] keyValue = d.Contains('=') ? d.Split('=') : d.Split(':');
                    string key = keyValue[0].ToUpper().Trim();

                    if (key.Contains("#EXT", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Id = keyValue[1].CleanString();
                    }
                    else if (key.Contains("BANDWIDTH", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Bandwidth = keyValue[1].CleanString();
                    }
                    else if (key.Contains("CODECS", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Codecs = keyValue[1].CleanString();
                    }
                    else if (key.Contains("RESOLUTION", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Resolution = keyValue[1].CleanString();
                    }
                    else if (key.Contains("NAME", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Name = keyValue[1].CleanString();
                    }
                    else if (key.Contains("LOGO", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.LogoUrl = keyValue[1].CleanString();
                    }
                    else if (key.Contains("ID", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.ProgramId = keyValue[1].CleanString();
                    }
                    else if (key.Contains("group-title", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] groupTitle = keyValue[1].CleanString().Split(",");

                        channel.Group = groupTitle[0].CleanString();

                        channel.Title = groupTitle.Length > 1 ? groupTitle[1].CleanString() : string.Empty;
                    }
                    else if (key.Contains("Group", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Group = keyValue[1].CleanString();
                    }
                    else if (key.Contains("Title", StringComparison.OrdinalIgnoreCase))
                    {
                        channel.Title = keyValue[1].CleanString();
                    }
                    else
                    {
                        _logger.Warning("{Key} is not supported. Moving next", keyValue[0]);
                    }
                }

                channel.Url = nextLine;
                playlist.Channels.Add(channel);
            }

            _playlist = playlist;
        }


        return _playlist;
    }

    public async Task<Playlist> RefreshPlaylist(string url)
    {
        _logger.Debug("Refresh playlist request in method {methodName}", nameof(RefreshPlaylist));
        _playlist = null;
        return await GetPlaylist(url);
    }

    public async Task<IEnumerable<Channel>> SearchPlaylist(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(_playlist);

        _logger.Debug("Searching playlist In method {methodName}", nameof(SearchPlaylist));

        List<Channel> channels = [.. _playlist.Channels.Where(w => w.Name.Contains(name))];

        return await Task.FromResult(channels);
    }
}