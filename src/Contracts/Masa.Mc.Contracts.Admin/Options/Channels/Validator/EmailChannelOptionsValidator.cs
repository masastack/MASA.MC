// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Options.Channels.Validator;

public class EmailChannelOptionsValidator : AbstractValidator<EmailChannelOptions>
{
    public EmailChannelOptionsValidator()
    {
        RuleFor(option => option.UserName).Required().WithMessage("Please enter email account");
        RuleFor(option => option.Password).Required().WithMessage("Please enter email password");
        RuleFor(option => option.Smtp).Required().WithMessage("Please enter smtp");
    }
}
