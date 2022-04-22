namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class GetMessageTaskInput : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public Guid? ChannelId { get; set; }
    public MessageEntityType? EntityType { get; set; }
    public bool? IsEnabled { get; set; }

    public GetMessageTaskInput(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTaskInput(string filter, Guid? channelId, MessageEntityType? entityType, bool? isEnabled, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        ChannelId = channelId;
        EntityType = entityType;
        IsEnabled = isEnabled;
    }
}
