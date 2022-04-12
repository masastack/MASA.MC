namespace Masa.Mc.Service.Admin.Application.Channels.Registers
{
    public class ChannelRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Channel, ChannelDto>().MapToConstructor(true);
            config.ForType<ChannelCreateUpdateDto, Channel>().MapToConstructor(true);
        }
    }
}
