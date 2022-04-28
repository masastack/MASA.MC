namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendTestMessageTaskInput
{
    public Guid Id { get; set; }
    public ReceiverDto Receivers { get; set; } = new();
}
