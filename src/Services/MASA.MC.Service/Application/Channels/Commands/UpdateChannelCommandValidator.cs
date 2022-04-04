namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public class UpdateChannelCommandValidator : AbstractValidator<UpdateChannelCommand>
{
    public UpdateChannelCommandValidator()
    {
        RuleFor(cmd => cmd.Channel.DisplayName).NotEmpty();
        RuleFor(cmd => cmd.Channel.Type).IsInEnum();
    }
    
}