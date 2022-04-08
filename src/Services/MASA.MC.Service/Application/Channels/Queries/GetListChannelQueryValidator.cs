using MASA.MC.Contracts.Admin.Dtos.Channels.Validator;

namespace MASA.MC.Service.Admin.Application.Channels.Queries;
public class GetListChannelQueryValidator : AbstractValidator<GetListChannelQuery>
{
    public GetListChannelQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetChannelInputValidator());
}