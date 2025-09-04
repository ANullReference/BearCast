using System.CommandLine;

namespace ConsoleApplication.Abstractions;

public interface ICommandLineHandler
{
    Task<RootCommand> CreateRootCommand();
}
