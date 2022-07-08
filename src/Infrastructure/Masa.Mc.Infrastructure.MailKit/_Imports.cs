// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using MailKit.Security;
global using MailKit.Net.Smtp;
global using Masa.Mc.Infrastructure.Email;
global using Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve;
global using System.Net.Mail;
global using Microsoft.Extensions.Options;
global using MimeKit;
global using SmtpClient = MailKit.Net.Smtp.SmtpClient;
global using Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve.Contributors;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;