namespace MASA.MC.Service.Admin.Application.Channels.Queries;
public class GetListChannelQueryValidator : AbstractValidator<GetListChannelQuery>
{
    public GetListChannelQueryValidator()
    {
        RuleFor(cmd => cmd.Input.Type).IsInEnum();
    }
}