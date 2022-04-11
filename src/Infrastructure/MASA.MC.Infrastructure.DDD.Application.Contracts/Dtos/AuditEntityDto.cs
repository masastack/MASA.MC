namespace MASA.MC.Infrastructure.DDD.Application.Contracts.Dtos;

public class AuditEntityDto<TKey, TUserId>: EntityDto<TKey>
{
    public TUserId Creator { get; set; } = default!;

    public DateTime CreationTime { get; set; }

    public TUserId Modifier { get; set; } = default!;

    public DateTime ModificationTime { get; set; }
}
