namespace Core.Abstraction;

public interface IFileManager
{
    bool Exists(string path);
    FileStream FileStream(string url, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
}
