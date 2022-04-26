namespace Masa.Mc.Caller.Services.MessageInfos;

public class MessageInfoService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageInfoService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/message-info";
    }

    public async Task<MessageInfoDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageInfoDto>($"{id}");
    }
}
