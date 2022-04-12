namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public class GetListChannelQueryValidator : AbstractValidator<GetListChannelQuery>
{
    public GetListChannelQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetChannelInputValidator());
}