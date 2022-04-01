﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Dtos
{
    public class PaginatedListDto<T>
    {
        public long Total { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<T> Result { get; set; } = default!;
        public PaginatedListDto(long total, int totalPages, IEnumerable<T> result)
        {
            Total = total;
            TotalPages = totalPages;
            Result = result;
        }
    }
}
