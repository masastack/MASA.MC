// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email;

public abstract class EmailSenderBase : IEmailSender
{
    protected readonly IEmailOptionsResolver _emailOptionsResolver;

    /// <summary>
    /// Constructor.
    /// </summary>
    protected EmailSenderBase(IEmailOptionsResolver emailOptionsResolver)
    {
        _emailOptionsResolver = emailOptionsResolver;
    }

    public virtual async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(new MailMessage
        {
            To = { to },
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml
        });
    }

    public virtual async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
    }

    public virtual async Task SendAsync(MailMessage mail, bool normalize = true)
    {
        if (normalize)
        {
            await NormalizeMailAsync(mail);
        }

        await SendEmailAsync(mail);
    }

    protected abstract Task SendEmailAsync(MailMessage mail);

    protected virtual async Task NormalizeMailAsync(MailMessage mail)
    {
        var options = await _emailOptionsResolver.ResolveAsync();
        if (mail.From == null || string.IsNullOrEmpty(mail.From.Address))
        {
            mail.From = new MailAddress(
                options.DefaultFromAddress,
                options.DefaultFromDisplayName,
                Encoding.UTF8
                );
        }

        if (mail.HeadersEncoding == null)
        {
            mail.HeadersEncoding = Encoding.UTF8;
        }

        if (mail.SubjectEncoding == null)
        {
            mail.SubjectEncoding = Encoding.UTF8;
        }

        if (mail.BodyEncoding == null)
        {
            mail.BodyEncoding = Encoding.UTF8;
        }
    }
}
