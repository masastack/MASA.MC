// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Xml.Linq;
global using Masa.Mc.Infrastructure.OptionsResolve;
global using Masa.Mc.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.Weixin.MiniProgram.Infrastructure.OptionsResolve;
global using Masa.Mc.Infrastructure.Weixin.MiniProgram.Model;
global using Masa.Mc.Infrastructure.Weixin.MiniProgram.Model.Response;
global using Masa.Mc.Infrastructure.Weixin.MiniProgram.Sender;
global using Masa.Mc.Infrastructure.Weixin.MiniProgram.Template;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using Senparc.Weixin.Entities;
global using Senparc.Weixin.Entities.TemplateMessage;
global using Senparc.Weixin.Tencent;
global using Senparc.Weixin.WxOpen.AdvancedAPIs;
global using Senparc.Weixin.WxOpen.Containers;
