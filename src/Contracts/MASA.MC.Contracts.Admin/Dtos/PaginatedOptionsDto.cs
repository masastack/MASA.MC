using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Dtos
{
    public class PaginatedOptionsDto
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Sorting { get; set; }
    }
}
