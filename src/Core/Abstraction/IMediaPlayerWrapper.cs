using Core.Domain;

namespace Core.Abstraction;

public interface IMediaPlayerWrapper
{
    Task<MediaPlayerStatusEnum> Play(string pathToFile);

    Task<MediaPlayerStatusEnum> Play(Channel channel);

    Task<MediaPlayerStatusEnum> Stop();
}
