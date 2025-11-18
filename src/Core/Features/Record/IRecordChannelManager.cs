using Core.Domain;

namespace Core.Features.Record;

public interface IRecordChannelManager
{
    Task<ResponseObject<string>> RecordChannel(Channel channel, DateTime startDate, DateTime endDate);
}
