// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Smtp;

public class SmtpEmailSender : EmailSenderBase, ISmtpEmailSender
{
    public SmtpEmailSender(IEmailOptionsResolver emailOptionsResolver) : base(emailOptionsResolver)
    {
    }

    public async Task<SmtpClient> BuildClientAsync()
    {
        var options = await base._emailOptionsResolver.ResolveAsync();
        var host = options.Host;
        var port = options.Port;

        var smtpClient = new SmtpClient(host, port);

        try
        {
            if (options.EnableSsl)
            {
                smtpClient.EnableSsl = true;
            }

            if (options.UseDefaultCredentials)
            {
                smtpClient.UseDefaultCredentials = true;
            }
            else
            {
                smtpClient.UseDefaultCredentials = false;

                var userName = options.UserName;
                if (!string.IsNullOrEmpty(userName))
                {
                    var password = options.Password;
                    var domain = options.Domain;
                    smtpClient.Credentials = !string.IsNullOrEmpty(domain)
                        ? new NetworkCredential(userName, password, domain)
                        : new NetworkCredential(userName, password);
                }
            }

            return smtpClient;
        }
        catch
        {
            smtpClient.Dispose();
            throw;
        }
    }

    protected override async Task SendEmailAsync(MailMessage mail)
    {
        using (var smtpClient = await BuildClientAsync())
        {
            await smtpClient.SendMailAsync(mail);
        }
    }
}
