using Core.Abstraction;

namespace Infrastructure;

public class FileManager : IFileManager
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public FileStream FileStream(string url, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
    {
        ArgumentNullException.ThrowIfNull(url, nameof(url));  
        return new FileStream(url, fileMode, fileAccess, fileShare);
    }
}
