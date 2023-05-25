// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Mc.Infrastructure.AppNotification.Model.Response;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Masa.Mc.Infrastructure.AppNotification.Infrastructure.OptionsResolve;
global using com.igetui.api.openservice;
global using com.igetui.api.openservice.igetui;
global using com.igetui.api.openservice.igetui.template;
global using Masa.Mc.Infrastructure.AppNotification.Getui;
global using Masa.Mc.Infrastructure.AppNotification.Infrastructure.OptionsResolve.Contributors;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using System.Collections.Concurrent;
global using System.Text.Json;
global using System.Net;
global using com.gexin.rp.sdk.dto;
global using Jiguang.JPush;
global using Jiguang.JPush.Model;
global using Masa.Mc.Infrastructure.AppNotification.JPush;
global using Newtonsoft.Json;