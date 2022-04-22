namespace Masa.Mc.Service.Admin.Application.MessageTasks.Registers
{
    public class MessageTaskRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageTask, MessageTaskDto>().MapToConstructor(true);
            config.ForType<MessageTaskCreateUpdateDto, MessageTask>().MapToConstructor(true);
        }
    }
}
