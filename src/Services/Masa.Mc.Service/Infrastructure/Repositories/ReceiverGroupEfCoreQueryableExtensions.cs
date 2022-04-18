namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public static class ReceiverGroupEfCoreQueryableExtensions
{
    public static IQueryable<ReceiverGroup> IncludeDetails(this IQueryable<ReceiverGroup> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }
        return queryable.Include(x => x.Users);
    }
}
