using Core.Domain;

namespace Core.Abstraction;

public interface IMediaPlayerWrapper
{
    Task<MediaPlayerStatusEnum> Play(Channel channel);

    Task<MediaPlayerStatusEnum> Play(string url);

    Task<MediaPlayerStatusEnum> Stop();
}
