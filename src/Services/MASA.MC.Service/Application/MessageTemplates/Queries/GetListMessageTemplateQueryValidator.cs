namespace MASA.MC.Service.Admin.Application.MessageTemplates.Queries;

public class GetListMessageTemplateQueryValidator : AbstractValidator<GetListMessageTemplateQuery>
{
    public GetListMessageTemplateQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetMessageTemplateInputValidator());
}