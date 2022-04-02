namespace MASA.MC.Contracts.Admin.Dtos;

public class AuditEntityDto<TKey, TUserId>
{
    public TKey Id { get; protected set; } = default!;
    public bool IsDeleted { get; protected set; }

    public TUserId Creator { get; protected set; } = default!;

    public DateTime CreationTime { get; protected set; }

    public TUserId Modifier { get; protected set; } = default!;

    public DateTime ModificationTime { get; protected set; }
}
