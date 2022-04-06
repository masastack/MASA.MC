namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator()
    {
        RuleFor(cmd => cmd.Channel.DisplayName).NotEmpty();
        RuleFor(cmd => cmd.Channel.Code).NotEmpty();
        RuleFor(cmd => cmd.Channel.Type).IsInEnum();
    }
}