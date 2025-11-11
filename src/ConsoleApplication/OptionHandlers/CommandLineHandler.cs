using ConsoleApplication.Abstractions;
using Core;
using Core.Abstraction;
using Core.Domain;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace ConsoleApplication.OptionCommandLineHandler;

/// <summary>
/// 
/// </summary>
public class CommandLineHandler(IPlayRequestManager playManager, IRequestManager requestManager, ILogger logger) : ICommandLineHandler
{
    private IPlayRequestManager _playManager = playManager;
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
                _logger.Information($"{channel.NAME} <--> {channel.Url}");
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
                _logger.Information($"{channel.NAME} <--> {channel.Url}");
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

        setM3u8Command.SetAction((parseResult) =>
        {
            // Debug logging
            _logger.Verbose("Parsing result tokens: {tokens}", 
            string.Join(", ", parseResult.Tokens.Select(t => t.Value)));

            var optionValue = parseResult.GetValue(m3u8UrlArgument);
             _logger.Verbose("Option value: '{value}'", optionValue ?? "NULL");
            
            _httpLink = optionValue ?? string.Empty;
            _logger.Verbose("Final httpLink: '{httpLink}'", _httpLink);
        });



        // Define the positional argument
        Argument<string> playUrlArgument = new("playUrl");
        // Define the command
        Command playUrlCommand = new (
            name: "-play_url",
            description: $"Play url with configured media player")
        {
            playUrlArgument
        };

        playUrlCommand.SetAction(async (parseResult) =>
        {
            // Debug logging
            _logger.Verbose("Parsing result tokens: {tokens}",
            string.Join(", ", parseResult.Tokens.Select(t => t.Value)));

            string urlOption = parseResult.GetValue(playUrlArgument) ?? string.Empty;
            _logger.Verbose("Option value: '{value}'", urlOption ?? "NULL");

            if (string.IsNullOrEmpty(urlOption))
            {
                //todo: log command error here maybe
                return;
            }

            Channel channelToPlay = new()
            {
                Url = urlOption
            };  

            CancellationToken cancellationToken = new();
            ResponseObject<int> responseObject = await _playManager.Play(channelToPlay, cancellationToken);
        });


        // Define the positional argument
        //Argument<string> stopArgument = new("stop");
        // Define the command
        Command stopCommand = new(
            name: "-stop",
            description: $"Stop");

        stopCommand.SetAction(async (parseResult) =>
        {
            // Debug logging
            _logger.Verbose("Parsing result tokens: {tokens}",
            string.Join(", ", parseResult.Tokens.Select(t => t.Value)));

            ResponseObject<int> responseObject = await _playManager.Stop();
        });

        rootCommand.Subcommands.Add(searchChannelCommand);
        rootCommand.Subcommands.Add(playListCommand);
        rootCommand.Subcommands.Add(setM3u8Command);
        rootCommand.Subcommands.Add(playUrlCommand);
        rootCommand.Subcommands.Add(stopCommand);

        return await Task.FromResult(rootCommand);
    }
}