namespace MASA.MC.Infrastructure.DDD.Application.Contracts.Dtos;

public class EntityDto<TKey>
{
    public TKey Id { get; protected set; } = default!;
}
