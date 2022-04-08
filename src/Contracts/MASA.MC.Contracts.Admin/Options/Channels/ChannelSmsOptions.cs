using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Options.Channels
{
    public class ChannelSmsOptions
    {
        public string AccessKeyId { get; set; }=string.Empty;

        public string AccessKeySecret { get; set; } = string.Empty;
    }
}
