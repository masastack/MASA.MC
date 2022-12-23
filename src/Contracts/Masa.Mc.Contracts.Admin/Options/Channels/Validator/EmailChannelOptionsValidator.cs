// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Options.Channels.Validator;

public class EmailChannelOptionsValidator : AbstractValidator<EmailChannelOptions>
{
    public EmailChannelOptionsValidator()
    {
        RuleFor(option => option.UserName).Required("EmailChannelUserNameRequired");
        RuleFor(option => option.Password).Required("EmailChannelPasswordRequired");
        RuleFor(option => option.Smtp).Required("EmailChannelSmtpRequired");
    }
}
