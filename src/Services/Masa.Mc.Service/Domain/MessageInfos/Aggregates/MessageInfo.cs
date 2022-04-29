namespace Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates;

public class MessageInfo : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public string Title { get; protected set; } = string.Empty;
    public string Content { get; protected set; } = string.Empty;
    public bool IsJump { get; protected set; }
    public string JumpUrl { get; protected set; } = string.Empty;
    public bool IsDeleted { get; protected set; }
}
