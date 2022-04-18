namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Queries;

public record GetListReceiverGroupQuery(GetReceiverGroupInput Input) : Query<PaginatedListDto<ReceiverGroupDto>>
{
    public override PaginatedListDto<ReceiverGroupDto> Result { get; set; } = default!;

}
