namespace MASA.MC.Contracts.Admin.Dtos;

public class EntityDto<TKey>
{
    public TKey Id { get; protected set; } = default!;
}
