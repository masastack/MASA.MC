namespace Masa.Mc.Infrastructure.DDD.Application.Contracts.Dtos;

public class EntityDto<TKey>
{
    public TKey Id { get; set; } = default!;
}
