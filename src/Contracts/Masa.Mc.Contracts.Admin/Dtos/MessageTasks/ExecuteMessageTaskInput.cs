namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class ExecuteMessageTaskInput
{
    public Guid MessageTaskId { get; set; }
    public ReceiverType ReceiverType { get; set; }
    public ExtraPropertyDictionary Receivers { get; set; }

    public ExtraPropertyDictionary SendingRules { get; set; }
}
