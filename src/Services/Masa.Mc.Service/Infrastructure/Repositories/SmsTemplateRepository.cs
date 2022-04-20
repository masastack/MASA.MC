namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class SmsTemplateRepository : Repository<McDbContext, SmsTemplate>, ISmsTemplateRepository
{
    public SmsTemplateRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }
}
