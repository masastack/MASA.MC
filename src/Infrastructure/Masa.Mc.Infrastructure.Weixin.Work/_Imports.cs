// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using System.Text.Json.Serialization;
global using Masa.Mc.Infrastructure.Weixin.Work.Model;
global using Masa.Mc.Infrastructure.Weixin.Work.Model.Response;
global using Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Senparc.Weixin.Work.AdvancedAPIs.Mass;
global using Senparc.Weixin.Work.Containers;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Senparc.Weixin.RegisterServices;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Hosting;
global using Senparc.Weixin.AspNet;