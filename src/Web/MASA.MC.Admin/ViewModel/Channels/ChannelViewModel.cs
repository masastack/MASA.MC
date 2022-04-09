using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Admin.ViewModel.Channels
{
    public class ChannelInfoViewModel
    {
        public string DisplayName { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public ChannelType Type { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsStatic { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; } = new();
    }
}
