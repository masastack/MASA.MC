using MASA.MC.Contracts.Admin.Dtos.Channels.Validator;

namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator() => RuleFor(cmd => cmd.Channel).SetValidator(new ChannelCreateUpdateDtoValidator());
}