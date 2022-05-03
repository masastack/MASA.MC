// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Utils.Caller.HttpClient;
global using Masa.Utils.Configuration.Json;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Options;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using System.Net;
global using Masa.Utils.Exceptions;
global using Microsoft.AspNetCore.Http;
global using Masa.Utils.Caller.Core;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Mc.Caller.Services.Channels;
global using Masa.Mc.Caller.Services.MessageTemplates;
global using Masa.Mc.Caller.Services.ReceiverGroups;
global using System.Reflection;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.Caller.Services.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.Caller.Services.MessageInfos;
