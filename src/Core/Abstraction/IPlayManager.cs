using Core.Domain;

namespace Core.Abstraction;

public interface IPlayManager
{
    Task<ResponseObject<int>> Play(string pathToFile);

    Task<ResponseObject<int>> PlayM3u8(string httpLink);
}
