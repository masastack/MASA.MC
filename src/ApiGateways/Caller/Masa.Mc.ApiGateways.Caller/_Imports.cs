﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Options;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using System.Net;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Mc.ApiGateways.Caller.Services.Channels;
global using Masa.Mc.ApiGateways.Caller.Services.MessageTemplates;
global using Masa.Mc.ApiGateways.Caller.Services.ReceiverGroups;
global using System.Reflection;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.ApiGateways.Caller.Services.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.ApiGateways.Caller.Services.MessageInfos;
global using Masa.Mc.Contracts.Admin.Dtos.MessageRecords;
global using Masa.Mc.ApiGateways.Caller.Services.MessageRecords;
global using Microsoft.AspNetCore.Mvc;
global using Masa.Mc.Contracts.Admin.Dtos;
global using Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;
global using Masa.Mc.ApiGateways.Caller.Services.WebsiteMessages;
global using Masa.Mc.Contracts.Admin.Dtos.Oss;
global using Masa.Mc.ApiGateways.Caller.Services.Oss;
global using Masa.Mc.Contracts.Admin.Dtos.Subjects;
global using Masa.BuildingBlocks.Service.Caller;
global using System.Text.Json;
global using System.Net.Http.Headers;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Microsoft.Extensions.Logging;
global using Masa.Contrib.Service.Caller;
global using IdentityModel.Client;
global using IdentityModel;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Net.Http;
global using Masa.Contrib.StackSdks.Caller;