using Masa.Mc.Infrastructure.Sms.Aliyun;
using Microsoft.Extensions.Options;

namespace Masa.Mc.Service.Admin.Services;

public class MessageTemplateService : ServiceBase
{
    public MessageTemplateService(IServiceCollection services) : base(services, "api/message-template")
    {
        MapPost(CreateAsync, string.Empty);
        MapPut(UpdateAsync, "{id}");
        MapDelete(DeleteAsync, "{id}");
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapGet(GetSmsTemplateAsync);
    }

    public async Task<PaginatedListDto<MessageTemplateDto>> GetListAsync(IEventBus eventbus, [FromQuery] ChannelType? channelType, [FromQuery] Guid? channelId, [FromQuery] MessageTemplateStatus? status, [FromQuery] MessageTemplateAuditStatus? auditStatus, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var input = new GetMessageTemplateInput(filter, channelType, channelId, status, auditStatus, startTime, endTime, sorting, page, pagesize);
        var query = new GetListMessageTemplateQuery(input);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTemplateDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTemplateQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] MessageTemplateCreateUpdateDto input)
    {
        var command = new CreateMessageTemplateCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] MessageTemplateCreateUpdateDto input)
    {
        var command = new UpdateMessageTemplateCommand(id, input);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteMessageTemplateCommand(id);
        await eventBus.PublishAsync(command);
    }

    public async Task<GetSmsTemplateDto> GetSmsTemplateAsync(IEventBus eventBus, [FromQuery] Guid channelId, [FromQuery] string templateCode)
    {
        var query = new GetSmsTemplateQuery(channelId, templateCode);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
