namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Registers
{
    public class MessageTemplateRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageTemplate, MessageTemplateDto>().MapToConstructor(true);
            config.ForType<MessageTemplateItemDto, MessageTemplateItem>().MapToConstructor(true);
            config.ForType<MessageTemplateCreateUpdateDto, MessageTemplate>().MapToConstructor(true);
        }
    }
}
