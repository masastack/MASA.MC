namespace Masa.Mc.Service.Admin.Application.MessageTasks.Registers
{
    public class MessageTaskRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageTask, MessageTaskDto>().MapToConstructor(true)
                .Map(dest => dest.Receivers, src => ExtraPropertyMapObj(src.Receivers))
                .Map(dest => dest.SendingRules, src => ExtensionPropertyHelper.ExtraPropertyMapToObj<SendingRuleDto>(src.SendingRules));
            config.ForType<MessageTaskCreateUpdateDto, MessageTask>().MapToConstructor(true)
                .Map(dest => dest.Receivers, src => ExtensionPropertyHelper.ObjMapToExtraProperty(src.Receivers))
                .Map(dest => dest.SendingRules, src => ExtensionPropertyHelper.ObjMapToExtraProperty(src.SendingRules));
        }
        private ReceiverDto ExtraPropertyMapObj(ExtraPropertyDictionary dic)
        {
            var extraPropertiesAsJson = JsonSerializer.Serialize(dic);
            if (string.IsNullOrEmpty(extraPropertiesAsJson) || extraPropertiesAsJson == "{}")
            {
                return new ReceiverDto();
            }
            return JsonSerializer.Deserialize<ReceiverDto>(extraPropertiesAsJson);
        }
    }
}
