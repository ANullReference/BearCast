using System.CommandLine;
using Core.Abstraction;
using Core.Domain;
using ConsoleApplication.Abstractions;

namespace ConsoleApplication.OptionCommandLineHandler;

/// <summary>
/// 
/// </summary>
public class CommandLineHandler(IPlayManager playManager, IRequestManager requestManager, ILogger logger) : ICommandLineHandler
{
    private IPlayManager _playManager = playManager;
    private IRequestManager _requestManager = requestManager;
    private ILogger _logger = logger;
    private string _httpLink = string.Empty;
    private Playlist _playList = new ();


    public async Task<RootCommand> CreateRootCommand()
    {
        RootCommand rootCommand = new();

        // Define an option for the root command
        Option<string> nameOption = new("-m3u8");//todo: set as const variable
        rootCommand.Options.Add(nameOption);

        // Set the action for the root command
        rootCommand.SetAction((parseResult) =>
        {
            _logger.Verbose("{httpLink} set", _httpLink);
            _httpLink = parseResult.GetValue(nameOption) ?? string.Empty;
        });

        Option<string> listChannelOption = new("-c", "--channels");
        Command playListCommand = new("-channels",  $"List channels for {_httpLink}")
        {
            listChannelOption
        };

        playListCommand.SetAction(async (parseResult) =>
        {
            _playList = await _requestManager.GetPlaylist(_httpLink);

            foreach (Channel channel in _playList.Channels)
            {
                _logger.Information(channel.NAME);
            }
        });

        rootCommand.Subcommands.Add(playListCommand);

        return await Task.FromResult(rootCommand);
    }
}