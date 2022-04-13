using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq;

public static class AbpPagingQueryableExtensions
{
    /// <summary>
    /// Used for paging with an <see cref="IPagedResultRequest"/> object.
    /// </summary>
    /// <param name="query">Queryable to apply paging</param>
    /// <param name="pagedResultRequest">An object implements <see cref="IPagedResultRequest"/> interface</param>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, PaginatedOptions pagedResultRequest)
    {
        return query.PageBy(pagedResultRequest.SkipCount, pagedResultRequest.MaxResultCount);
    }
}
