namespace MASA.MC.Admin.Profiles;

public class ChannelProfile : Profile
{
    public ChannelProfile()
    {
        CreateMap<ChannelDto, ChannelInfoViewModel>();
        CreateMap<ChannelInfoViewModel, ChannelCreateUpdateDto>();
        CreateMap<ChannelDto, ChannelCreateUpdateDto>();
    }
}
