// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Masa.Mc.Infrastructure.Sms.YunMas;
global using Masa.Mc.Infrastructure.Sms.Model.Response;
global using AliyunClient = AlibabaCloud.SDK.Dysmsapi20170525.Client;
global using AliyunConfig = AlibabaCloud.OpenApiClient.Models.Config;
global using AliyunSendSmsRequest = AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest;
global using AlibabaCloud.SDK.Dysmsapi20170525.Models;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Masa.Mc.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.OptionsResolve;
global using Masa.Mc.Infrastructure.Sms.Aliyun;
global using Masa.Mc.Infrastructure.Sms.Model.Response.SendSms;
global using Masa.Mc.Infrastructure.Sms.Model.Response.SmsTemplate;
global using System.Collections.Concurrent;