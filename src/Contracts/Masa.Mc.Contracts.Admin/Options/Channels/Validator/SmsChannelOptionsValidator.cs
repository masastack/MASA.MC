﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Options.Channels.Validator;

public class SmsChannelOptionsValidator : AbstractValidator<SmsChannelOptions>
{
    public SmsChannelOptionsValidator()
    {
        RuleFor(option => option.AccessKeyId).Required().WithMessage("Please enter accessKeyId");
        RuleFor(option => option.AccessKeySecret).Required().WithMessage("Please enter accessKeySecret");
    }
}
