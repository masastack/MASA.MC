using MASA.MC.Contracts.Admin.Enums.NotificationTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Dtos.NotificationTemplates
{
    public class NotificationTemplateCreateUpdateDto
    {
        public virtual NotificationTemplateStatus Status { get; set; }
    }
}
