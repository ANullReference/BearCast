using System.Threading.Channels;
using Core.Domain;
using Channel = Core.Domain.Channel;

namespace Core.Abstraction;


public interface IMediaPlayerWrapper
{
    Task<MediaPlayerStatusEnum> Play(string pathToFile);
    Task<MediaPlayerStatusEnum> Play(Channel channel);
    Task<MediaPlayerStatusEnum> Stop();
}