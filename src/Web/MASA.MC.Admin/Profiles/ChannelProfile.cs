using AutoMapper;
using MASA.MC.Contracts.Admin.Dtos.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Admin.Profiles;

public class ChannelProfile : Profile
{
    public ChannelProfile()
    {

        CreateMap<ChannelDto, ChannelCreateUpdateDto>();
    }
}
