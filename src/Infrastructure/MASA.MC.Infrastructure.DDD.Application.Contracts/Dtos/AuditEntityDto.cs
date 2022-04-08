namespace MASA.MC.Infrastructure.DDD.Application.Contracts.Dtos;

public class AuditEntityDto<TKey, TUserId>: EntityDto<TKey>
{
    public bool IsDeleted { get; set; }

    public TUserId Creator { get; set; } = default!;

    public DateTime CreationTime { get; set; }

    public TUserId Modifier { get; set; } = default!;

    public DateTime ModificationTime { get; set; }
}
