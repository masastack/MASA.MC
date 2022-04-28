namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class WithdrawnMessageTaskHistoryInput
{
    public Guid MessageTaskId { get; set; }
    public Guid HistoryId { get; set; }
}
