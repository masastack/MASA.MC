namespace MASA.MC.Infrastructure.DDD.Application.Contracts.Dtos;

public class AuditEntityDto<TKey, TUserId>: EntityDto<TKey>
{
    public bool IsDeleted { get; protected set; }

    public TUserId Creator { get; protected set; } = default!;

    public DateTime CreationTime { get; protected set; }

    public TUserId Modifier { get; protected set; } = default!;

    public DateTime ModificationTime { get; protected set; }
}
