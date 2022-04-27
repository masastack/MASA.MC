namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendMessageTaskInput
{
    public Guid Id { get; set; }
    public ReceiverType ReceiverType { get; set; }
    public string Sign { get; set; } = string.Empty;
    public DateTime? SendTime { get; set; }
    public ReceiverDto Receivers { get; set; } = new();
    public SendingRuleDto SendingRules { get; set; } = new();
    public ExtraPropertyDictionary Variables { get; set; } = new();
}
