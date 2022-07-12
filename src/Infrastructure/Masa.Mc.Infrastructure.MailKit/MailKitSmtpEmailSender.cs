// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.MailKit;

public class MailKitSmtpEmailSender : EmailSenderBase, IMailKitSmtpEmailSender
{
    protected MailKitOptions MailKitOptions { get; }

    public MailKitSmtpEmailSender(IEmailOptionsResolver emailOptionsResolver, IOptions<MailKitOptions> mailKitConfiguration) : base(emailOptionsResolver)
    {
        MailKitOptions = mailKitConfiguration.Value;
    }

    protected override async Task SendEmailAsync(MailMessage mail)
    {
        using (var client = await BuildClientAsync())
        {
            var message = MimeMessage.CreateFromMailMessage(mail);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public async Task<SmtpClient> BuildClientAsync()
    {
        var client = new SmtpClient();

        try
        {
            await ConfigureClient(client);
            return client;
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    protected virtual async Task ConfigureClient(SmtpClient client)
    {
        var options = await base._emailOptionsResolver.ResolveAsync();

        await client.ConnectAsync(
            options.Host,
            options.Port,
            await GetSecureSocketOption()
        );

        if (options.UseDefaultCredentials)
        {
            return;
        }

        await client.AuthenticateAsync(
            options.UserName,
            options.Password
        );
    }

    protected virtual async Task<SecureSocketOptions> GetSecureSocketOption()
    {
        var options = await base._emailOptionsResolver.ResolveAsync();

        if (MailKitOptions.SecureSocketOption.HasValue)
        {
            return MailKitOptions.SecureSocketOption.Value;
        }

        return options.EnableSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;
    }
}
