using Core.Domain;

namespace Core.Abstraction;

public interface IMediaPlayerWrapper
{
    Task Play(string pathToFile);

    Task PlayM3u8(string httpLink);
}
