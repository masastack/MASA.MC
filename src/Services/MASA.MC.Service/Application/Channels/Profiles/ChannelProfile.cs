﻿namespace MASA.MC.Service.Admin.Application.Channels.Profiles;

public class ChannelProfile : Profile
{
    public ChannelProfile()
    {
        CreateMap<Channel, ChannelDto>();
        CreateMap<ChannelCreateUpdateDto, Channel>();
    }
}
