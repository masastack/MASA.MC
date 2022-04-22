namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetListMessageTaskQuery(GetMessageTaskInput Input) : Query<PaginatedListDto<MessageTaskDto>>
{
    public override PaginatedListDto<MessageTaskDto> Result { get; set; } = default!;

}
