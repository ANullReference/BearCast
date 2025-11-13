using Core.Domain;

namespace Core.Abstraction;

public interface IPlayRequestManager
{
    Task<ResponseObject<int>> Play(string pathToFile);
    Task<ResponseObject<int>> Play(Channel channel);
    Task<ResponseObject<int>> Record(Channel channel);
    Task<ResponseObject<int>> Pause(Channel channel);
    Task<ResponseObject<int>> Stop();
}
