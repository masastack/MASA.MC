using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASA.MC.Contracts.Admin.Dtos.Channels.Validator;

public class GetChannelInputValidator : AbstractValidator<GetChannelInput>
{
    public GetChannelInputValidator()
    {
        RuleFor(input => input.Type).IsInEnum();
    }
}
