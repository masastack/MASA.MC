namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class SendTestMessageTaskInputValidator : AbstractValidator<SendTestMessageTaskInput>
{
    public SendTestMessageTaskInputValidator()
    {
        RuleFor(input => input.Receivers).Must(x => x.Items.Any());
    }
}