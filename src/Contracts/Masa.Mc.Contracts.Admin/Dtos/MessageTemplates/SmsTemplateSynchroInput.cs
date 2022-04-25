namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class SmsTemplateSynchroInput
{
    public Guid ChannelId { get; set; }

    public SmsTemplateSynchroInput(Guid channelId)
    {
        ChannelId = channelId;
    }
}
