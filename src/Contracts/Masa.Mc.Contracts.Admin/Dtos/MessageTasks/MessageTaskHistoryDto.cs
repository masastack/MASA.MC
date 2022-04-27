namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskHistoryDto : AuditEntityDto<Guid, Guid>
{
    public Guid MessageTaskId { get; set; }

    public ReceiverType ReceiverType { get; set; }

    public MessageTaskHistoryStatus Status { get; set; }

    public ReceiverDto Receivers { get; set; } = new();

    public SendingRuleDto SendingRules { get; set; } = new();

    public DateTime? SendTime { get; set; }

    public DateTime? CompletionTime { get; set; }

    public DateTime? WithdrawTime { get; set; }

    public string Sign { get; set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; set; } = new();
}
