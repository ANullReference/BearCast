using Core.Abstraction;
using Core.Domain;
using Core.Features.Record;

namespace Infrastructure.Features;

public class RecordChannelManager : IRecordChannelManager
{
    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly ILogger _logger;
    private readonly IMediaPlayerWrapper _mediaPlayerWrapper;

    public RecordChannelManager(IHttpClientFactory httpClientFactory, IMediaPlayerWrapper mediaPlayerWrapper, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _mediaPlayerWrapper = mediaPlayerWrapper;
        _logger = logger;
    }

    public async Task<ResponseObject<string>> RecordChannel(Channel channel, DateTime startDate, DateTime endDate)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient(Constants.TvHttpClientUrl); // Use the factory to create an HttpClient instance

        return await Task.FromResult( new ResponseObject<string>(ResponseEnum.Success,""));
    }
}
