namespace Masa.Mc.Service.Admin.Application.MessageInfos.Queries;

public record GetMessageInfoQuery(Guid MessageInfoId) : Query<MessageInfoDto>
{
    public override MessageInfoDto Result { get; set; } = new();
}
