using MASA.MC.Contracts.Admin.Dtos.Channels.Validator;

namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public class UpdateChannelCommandValidator : AbstractValidator<UpdateChannelCommand>
{
    public UpdateChannelCommandValidator() => RuleFor(cmd => cmd.Channel).SetValidator(new ChannelCreateUpdateDtoValidator());
}