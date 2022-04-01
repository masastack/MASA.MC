using MASA.MC.Contracts.Admin.Enums.Channels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Dtos.Channels
{
    public class GetChannelInput : PaginatedOptionsDto
    {
        public ChannelType Type { get; set; }
        public GetChannelInput(int page, int pageSize, ChannelType type)
        {
            Page = page;
            PageSize = pageSize;
            Type = type;
        }
    }
}
