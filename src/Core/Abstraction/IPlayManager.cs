using Core.Domain;

namespace Core.Abstraction;

public interface IPlayRequestManager
{
    Task<ResponseObject<int>> Play(string pathToFile, CancellationToken cancellationToken);
    Task<ResponseObject<int>> Play(Channel channel, CancellationToken cancellationToken);
    Task<ResponseObject<int>> Record(Channel channel, CancellationToken cancellationToken);
    Task<ResponseObject<int>> Pause(Channel channel, CancellationToken cancellationToken);
    Task<ResponseObject<int>> Stop();
}
