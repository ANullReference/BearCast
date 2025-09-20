using System.CommandLine;
using Core.Abstraction;
using Core.Domain;
using ConsoleApplication.Abstractions;
using System.CommandLine.NamingConventionBinder;

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
    private Playlist _playList = new();


    public async Task<RootCommand> CreateRootCommand()
    {
        RootCommand rootCommand = new();

        // Define an option for the root command
         Option<string> mainOption = new("--bear");//todo: set as const variable
         rootCommand.Options.Add(mainOption);

        Option<string> channelOption = new("ChannelList", "-channel");
        Command playListCommand = new("-channel_list", $"Channel lists")
        {
            channelOption
        };
        playListCommand.SetAction(async (parseResult) =>
        {
            _playList = await _requestManager.GetPlaylist(_httpLink);

            foreach (Channel channel in _playList.Channels)
            {
                _logger.Information(channel.NAME);
            }
        });

        Argument<string> searchChannelArgument = new ("searched term");
        Option<string> channelSearchOption = new ("ChannelSearch", "-cs");
        Command searchChannelCommand = new ("-channel_search", $"Search channels for {_httpLink}")
        {
            channelSearchOption
        };

        searchChannelCommand.Arguments.Add(searchChannelArgument);
        searchChannelCommand.SetAction(async (parseResult) =>
        {
            string searchedTerm = parseResult.GetValue(channelSearchOption) ?? string.Empty;
            IEnumerable<Channel> channels = await _requestManager.SearchPlaylist(searchedTerm);

            foreach (Channel channel in channels)
            {
                _logger.Information(channel.NAME);
            }
        });

        // Define the positional argument
        Argument<string> m3u8UrlArgument = new("m3u8Url");
        // Define the command
        Command setM3u8Command = new (
            name: "-set_m3u8",
            description: $"Build PlayList with the specified URL")
        {
            m3u8UrlArgument
        };

        setM3u8Command.SetAction( (parseResult) =>
        {
            // Debug logging
            _logger.Verbose("Parsing result tokens: {tokens}", 
                string.Join(", ", parseResult.Tokens.Select(t => t.Value)));

            var optionValue = parseResult.GetValue(m3u8UrlArgument);
             _logger.Verbose("Option value: '{value}'", optionValue ?? "NULL");
            
            _httpLink = optionValue ?? string.Empty;
            _logger.Verbose("Final httpLink: '{httpLink}'", _httpLink);
        });
        
        rootCommand.Subcommands.Add(searchChannelCommand);
        rootCommand.Subcommands.Add(playListCommand);
        rootCommand.Subcommands.Add(setM3u8Command);

        return await Task.FromResult(rootCommand);
    }
}