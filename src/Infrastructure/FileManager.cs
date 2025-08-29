using System.Reflection;
using Core.Abstraction;

namespace Infrastructure;

public class FileManager : IFileManager
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }
}
