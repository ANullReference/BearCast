using Core.Domain;

namespace Core.Abstraction;

public interface IMediaPlayerWrapper
{
    Task<MediaPlayerStatusEnum> Play(string pathToFile, CancellationToken cancellationToken);

    Task<MediaPlayerStatusEnum> Play(Channel channel, CancellationToken cancellationToken);
}
