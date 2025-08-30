using System.CommandLine;

namespace ConsoleApplication.Options;

public class BearCastOptions
{
    public RootCommand GetOptions()
    {
        RootCommand rootCommand = new()
        {
            new Option<string>("--play", "-p"),
            new Option<string>("--m3u8", "-m")
        };

        rootCommand.Description = "This application will run videos from m3u8 links using command line.";
        //rootCommand.Handler

        return rootCommand;
    }
}
