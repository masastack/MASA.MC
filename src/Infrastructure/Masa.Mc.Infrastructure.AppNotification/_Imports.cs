// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using com.igetui.api.openservice;
global using com.igetui.api.openservice.igetui;
global using com.igetui.api.openservice.igetui.template;
global using Jiguang.JPush;
global using Jiguang.JPush.Model;
global using Masa.BuildingBlocks.Caching;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Mc.Infrastructure.AppNotification.Getui;
global using Masa.Mc.Infrastructure.AppNotification.iOS;
global using Masa.Mc.Infrastructure.AppNotification.Honor;
global using Masa.Mc.Infrastructure.AppNotification.Huawei;
global using Masa.Mc.Infrastructure.AppNotification.JPush;
global using Masa.Mc.Infrastructure.AppNotification.Oppo;
global using Masa.Mc.Infrastructure.AppNotification.Vivo;
global using Masa.Mc.Infrastructure.AppNotification.Xiaomi;
global using Masa.Mc.Infrastructure.AppNotification.Model.Response;
global using Masa.Mc.Infrastructure.Cache;
global using Masa.Mc.Infrastructure.OptionsResolve;
global using Masa.Mc.Infrastructure.OptionsResolve.Contributors;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Newtonsoft.Json;
global using System.Collections.Concurrent;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;