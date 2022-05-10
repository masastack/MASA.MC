// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve.Contributors;

public class ConfigurationOptionsResolveContributor : IEmailOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "Configuration";
    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(EmailOptionsResolveContext context)
    {
        context.Options = context.ServiceProvider.GetRequiredService<IOptions<SmtpEmailOptions>>().Value;

        return Task.CompletedTask;
    }
}