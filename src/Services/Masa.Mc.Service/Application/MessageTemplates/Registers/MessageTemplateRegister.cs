namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Registers
{
    public class MessageTemplateRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageTemplate, MessageTemplateDto>().MapToConstructor(true);
            config.ForType<MessageTemplateItem, MessageTemplateItemDto>().MapToConstructor(true);
        }
    }
}
