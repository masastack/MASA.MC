// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

// System Namespaces
global using System.Collections.Concurrent;
global using System.Reflection;
global using System.Text;

// Microsoft Namespaces
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.JSInterop;

// Third-party Libraries
global using BlazorDownloadFile;
global using FluentValidation;
global using Mapster;

// MASA Building Blocks and Contrib Packages
global using Masa.BuildingBlocks.StackSdks.Auth;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.Contrib.StackSdks.Config;

// MASA Blazor Components
global using Masa.Blazor;
global using Masa.Blazor.Components.Editor;
global using Masa.Blazor.Presets;
global using Masa.Stack.Components;
global using Masa.Stack.Components.Extensions;

// MASA MC Contracts, Infrastructure, Domain, Services, and Application
global using Masa.Mc.ApiGateways.Caller;
global using Masa.Mc.ApiGateways.Caller.Services.Channels;
global using Masa.Mc.ApiGateways.Caller.Services.MessageTemplates;
global using Masa.Mc.ApiGateways.Caller.Services.ReceiverGroups;
global using Masa.Mc.ApiGateways.Caller.Services.MessageTasks;
global using Masa.Mc.ApiGateways.Caller.Services.MessageInfos;
global using Masa.Mc.ApiGateways.Caller.Services.MessageRecords;
global using Masa.Mc.ApiGateways.Caller.Services.Oss;
global using Masa.Mc.Contracts.Admin.Dtos;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.Contracts.Admin.Dtos.MessageRecords;
global using Masa.Mc.Contracts.Admin.Dtos.Subjects;
global using Masa.Mc.Domain.Shared.Channels;
global using Masa.Mc.Domain.Shared.MessageTemplates;
global using Masa.Mc.Domain.Shared.ReceiverGroups;
global using Masa.Mc.Domain.Shared.MessageTasks;
global using Masa.Mc.Domain.Shared.MessageRecords;
global using Masa.Mc.Domain.Shared.Consts;
global using Masa.Mc.Infrastructure.Common.Helper;
global using Masa.Mc.Infrastructure.Common.Extensions;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Masa.Mc.Data;

// MASA MC Web Admin Pages and Components
global using Masa.Mc.Web.Admin.Global;
global using Masa.Mc.Web.Admin.Pages.Channels.Modules;
global using Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;
global using Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;
global using Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;
global using Masa.Mc.Web.Admin.Pages.MessageRecords.Modules;
global using Masa.Mc.Web.Admin.Components.Modules.Subjects;
global using Masa.Mc.Web.Admin.Model;

// Validators
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;
global using Masa.Mc.Contracts.Admin.Options.Channels;