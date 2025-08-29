using System.ComponentModel;

namespace Core.Domain;



/// #EXTM3U
/// #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2149280,CODECS=\"mp4a.40.2,avc1.64001f\",RESOLUTION=1280x720,NAME=\"720\"
/// url_0/193039199_mp4_h264_aac_hd_7.m3u8
public class Playlist
{
    public string ExtM3U { get; set; } = string.Empty;

    public List<Channel> Channels { get; set; } = [];
}


public class Channel
{
    [DisplayName("#EXT-X-STREAM-INF:PROGRAM-ID")]
    public string ProgramId { get; set; } = string.Empty;

    [DisplayName("BANDWIDTH")]
    public string Bandwidth { get; set; } = string.Empty;

    [DisplayName("CODECS")]
    public string Codecs { get; set; } = string.Empty;


    [DisplayName("RESOLUTION")]
    public string Resolution { get; set; } = string.Empty;


    [DisplayName("NAME")]
    public string NAME { get; set; } = string.Empty;
    
    [DisplayName("URL")]
    public string Url { get; set; } = string.Empty;
}




