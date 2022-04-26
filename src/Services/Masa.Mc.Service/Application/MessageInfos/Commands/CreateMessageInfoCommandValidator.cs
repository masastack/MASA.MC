namespace Masa.Mc.Service.Admin.Application.MessageInfos.Commands;

public class CreateMessageInfoCommandValidator : AbstractValidator<CreateMessageInfoCommand>
{
    public CreateMessageInfoCommandValidator() => RuleFor(cmd => cmd.MessageInfo).SetValidator(new MessageInfoCreateUpdateDtoValidator());
}