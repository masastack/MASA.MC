using MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Repositories;
using MASA.MC.Service.Admin.Infrastructure.EntityFrameworkCore;

namespace MASA.MC.Service.Admin.Infrastructure.Repositories
{
    public class NotificationTemplateRepository : Repository<MCDbContext, NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(MCDbContext context, IUnitOfWork unitOfWork)
            : base(context, unitOfWork)
        {
        }
        public async Task<IQueryable<NotificationTemplate>> GetQueryableAsync()
        {
            return await Task.FromResult(_context.Set<NotificationTemplate>().AsQueryable());
        }
    }
}
