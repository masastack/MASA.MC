namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Queries;

public record GetReceiverGroupQuery(Guid ReceiverGroupId) : Query<ReceiverGroupDto>
{
    public override ReceiverGroupDto Result { get; set; } = new();
}
