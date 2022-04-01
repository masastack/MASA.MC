using Masa.Contrib.ReadWriteSpliting.Cqrs.Queries;
using MASA.MC.Contracts.Admin.Dtos;
using MASA.MC.Contracts.Admin.Dtos.Channels;

namespace MASA.MC.Service.Admin.Application.Channels.Queries
{
    public record GetListChannelQuery(GetChannelInput Input) : Query<PaginatedListDto<ChannelDto>>
    {

        public override PaginatedListDto<ChannelDto> Result { get; set; } = default!;

    }
}
