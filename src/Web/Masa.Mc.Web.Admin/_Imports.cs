﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using BlazorComponent;
global using BlazorComponent.I18n;
global using Masa.Blazor;
global using Masa.Mc.Web.Admin.Global;
global using Masa.Mc.Web.Admin.Global.Config;
global using Masa.Mc.Web.Admin.Global.Nav.Model;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Http;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Net.Http.Json;
global using System.Reflection;
global using System.Text.Json;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Contracts.Admin.Options.Channels;
global using Masa.Mc.Infrastructure.ObjectExtending;
global using Masa.Mc.Infrastructure.ObjectExtending.ObjectExtending;
global using Masa.Mc.Web.Admin.Pages.Channels.Modules;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Mapster;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;
global using Masa.Mc.Contracts.Admin.Enums.MessageTemplates;
global using Masa.Mc.Infrastructure.Common.Helper;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;
global using Masa.Mc.Contracts.Admin.Enums.ReceiverGroups;
global using Masa.Mc.Web.Admin.Components.Pagination;
global using Masa.Mc.ApiGateways.Caller;
global using Masa.Mc.ApiGateways.Caller.Services.Channels;
global using Masa.Mc.ApiGateways.Caller.Services.MessageTemplates;
global using Masa.Mc.ApiGateways.Caller.Services.ReceiverGroups;
global using FluentValidation;
global using Masa.Mc.Infrastructure.Common.Extensions;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.ApiGateways.Caller.Services.MessageTasks;
global using Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos;
global using Masa.Mc.ApiGateways.Caller.Services.MessageInfos;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.ApiGateways.Caller.Services.MessageRecords;
global using Masa.Mc.Contracts.Admin.Dtos.MessageRecords;
global using Masa.Mc.Web.Admin.Pages.MessageRecords.Modules;
global using Masa.Mc.Web.Admin.Components.Modules.Subjects;
global using System.Text;
global using Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;
global using Masa.Mc.ApiGateways.Caller.Services.WebsiteMessages;
global using Microsoft.AspNetCore.SignalR.Client;
global using Masa.Mc.Contracts.Admin.Consts;
global using Masa.Mc.Web.Admin.Store;
global using BlazorDownloadFile;
global using Masa.Mc.Web.Admin.Components.Messages;
global using Microsoft.JSInterop;
global using Masa.Mc.ApiGateways.Caller.Services.Oss;
global using Masa.Blazor.Components.Editor;
global using Masa.Mc.ApiGateways.Caller.Services.Subjects;
global using Masa.Mc.Contracts.Admin.Dtos.Subjects;
global using Masa.Mc.Web.Admin.Components.Modules.MessageTasks;
global using Masa.Mc.Web.Admin.Data.Shared.Favorite;
global using Masa.Mc.Web.Admin.Data.Others.AccountSettings.Dto;
global using Masa.Mc.Web.Admin.Data.Base;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Utils.Data.Elasticsearch;
global using Masa.Contrib.SearchEngine.AutoComplete;