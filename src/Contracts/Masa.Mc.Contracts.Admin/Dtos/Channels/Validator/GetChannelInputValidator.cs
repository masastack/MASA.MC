namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class GetChannelInputValidator : AbstractValidator<GetChannelInput>
{
    public GetChannelInputValidator()
    {
        RuleFor(input => input.Type).IsInEnum();
    }
}
