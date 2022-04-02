using MASA.MC.Contracts.Admin.Dtos.Channels;
using MASA.MC.Service.Admin.Domain.Channels.Aggregates;

namespace MASA.MC.Service.Admin.Application.Channels.Profiles;

public class ChannelProfile : Profile
{
    public ChannelProfile()
    {

        CreateMap<Channel, ChannelDto>();
        CreateMap<ChannelCreateUpdateDto, Channel>();
    }
}
