namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class ChannelCreateUpdateDtoValidator : AbstractValidator<ChannelCreateUpdateDto>
{
    public ChannelCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).NotEmpty();
        RuleFor(input => input.Code).NotEmpty();
        RuleFor(input => input.Type).IsInEnum();
    }
}
