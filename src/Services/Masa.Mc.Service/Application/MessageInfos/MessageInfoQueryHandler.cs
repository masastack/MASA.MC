namespace Masa.Mc.Service.Admin.Application.MessageInfos;

public class MessageInfoQueryHandler
{
    private readonly IMessageInfoRepository _repository;

    public MessageInfoQueryHandler(IMessageInfoRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageInfoQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageInfoId);
        if (entity == null)
            throw new UserFriendlyException("messageInfo not found");
        query.Result = entity.Adapt<MessageInfoDto>();
    }
}
