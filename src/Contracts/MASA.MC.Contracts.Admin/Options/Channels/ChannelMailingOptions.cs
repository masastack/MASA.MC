using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Options.Channels
{
    public class ChannelMailingOptions
    {
        public string UserName { get; set; }=string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Smtp { get; set; } = string.Empty;
        public bool SSL { get; set; }
        public int Port { get; set; }
    }
}
