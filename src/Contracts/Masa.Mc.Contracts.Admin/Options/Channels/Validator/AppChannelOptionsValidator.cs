// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Options.Channels.Validator;

public class AppChannelOptionsValidator : AbstractValidator<AppChannelOptions>
{
    public AppChannelOptionsValidator()
    {
        RuleFor(option => option.Provider).Required();
    }
}
