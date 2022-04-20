namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Registers;

public class ReceiverGroupRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<ReceiverGroup, ReceiverGroupDto>().MapToConstructor(true);
        config.ForType<ReceiverGroupUserDto, ReceiverGroupUser>().MapToConstructor(true);
        config.ForType<ReceiverGroupCreateUpdateDto, ReceiverGroup>().MapToConstructor(true).Ignore(x => x.Items);
        config.ForType<ReceiverGroupItemDto, ReceiverGroupItem>().MapToConstructor(true);
    }
}
