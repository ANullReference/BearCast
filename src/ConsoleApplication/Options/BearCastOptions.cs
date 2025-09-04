using System.CommandLine;
using System.CommandLine.Invocation;
namespace ConsoleApplication.Options;

public class BearCastOptions
{
    private string httpLink = string.Empty;

    public RootCommand GetRootCommand()
    {

        // rootCommand.SetAction((action) =>
        // {
        //     httpLink = action.GetValue<string>("--m3u8") ?? string.Empty;
        //     //httpLink = action.CommandResult.GetResult<ParseResult>( a => a. );
        //     Console.WriteLine(action);
        // });

        Command setm3u8Command = new("--m3u8", "http m3u8 link");
        setm3u8Command.SetAction(parseResult =>
        {
            var name = parseResult.GetValue<string>("--m3u8") ?? string.Empty;
            Console.WriteLine($"Hello {name}!");
        });


        


        RootCommand rootCommand = new();
        rootCommand.Description = "This application will run http videos from m3u8 links using command line.";

        
        


        rootCommand.Add(setm3u8Command);


        return rootCommand;
    }
}
