using System.CommandLine;
using ConsoleApplication.Options;
using Core.Abstraction;

using System.CommandLine.Invocation;

namespace ConsoleApplication.OptionHandlers;

/// <summary>
/// 
/// </summary>
public class Handlers(IPlayManager playManager, IRequestManager requestManager)
{
    private IPlayManager _playManager = playManager;
    private IRequestManager _requestManager = requestManager;




    public void HandleConsoleArguments()
    {
        BearCastOptions bearCastOptions = new();
        RootCommand rootCommand = bearCastOptions.GetOptions();


        //Command command = new Command()

    }


}
