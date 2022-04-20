namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public static class MessageTemplateEfCoreQueryableExtensions
{
    public static IQueryable<MessageTemplate> IncludeDetails(this IQueryable<MessageTemplate> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }
        return queryable.Include(x => x.Items);
    }
}
