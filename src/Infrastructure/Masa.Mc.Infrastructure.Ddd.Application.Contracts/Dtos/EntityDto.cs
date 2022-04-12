namespace Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;

public class EntityDto<TKey>
{
    public TKey Id { get; set; } = default!;
}
