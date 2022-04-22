namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetMessageTaskQuery(Guid MessageTaskId) : Query<MessageTaskDto>
{
    public override MessageTaskDto Result { get; set; } = new();
}
