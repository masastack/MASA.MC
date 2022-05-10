// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email;

public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        bool isBodyHtml = true
    );

    Task SendAsync(
        string from,
        string to,
        string subject,
        string body,
        bool isBodyHtml = true
    );

    Task SendAsync(
        MailMessage mail,
        bool normalize = true
    );
}
