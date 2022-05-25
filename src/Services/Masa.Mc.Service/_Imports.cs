﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using System.Text.Json;
global using Dapr.Actors;
global using Dapr.Actors.Client;
global using Dapr.Actors.Runtime;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Configuration;
global using Masa.Contrib.Data.UoW.EF;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Events;
global using Masa.Contrib.Ddd.Domain.Repository.EF;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.Events.Enums;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;
global using Masa.Contrib.ReadWriteSpliting.Cqrs.Commands;
global using Masa.Contrib.ReadWriteSpliting.Cqrs.Queries;
global using Masa.Contrib.Service.MinimalAPIs;
global using Masa.Utils.Exceptions.Extensions;
global using Masa.Mc.Service.Infrastructure;
global using Masa.Mc.Service.Infrastructure.Middleware;
global using Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;
global using Masa.Mc.Service.Admin.Domain.Channels.Aggregates;
global using Masa.Mc.Service.Admin.Application.Channels.Commands;
global using Masa.Mc.Service.Admin.Domain.Channels.Repositories;
global using Masa.Mc.Service.Admin.Domain.Channels.Services;
global using Masa.Mc.Service.Admin.Application.Channels.Queries;
global using System.Linq.Expressions;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
global using Masa.Mc.Contracts.Admin.Enums.MessageTemplates;
global using Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;
global using Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Infrastructure.ObjectExtending;
global using Masa.Mc.Service.Admin.Domain.MessageTemplates.Repositories;
global using Masa.Mc.Infrastructure.EntityFrameworkCore.EntityFrameworkCore.ValueConverters;
global using Masa.Mc.Service.Admin.Domain.Consts;
global using System.Collections.Concurrent;
global using Microsoft.AspNetCore.Mvc;
global using Mapster;
global using MapsterMapper;
global using System.Reflection;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;
global using Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;
global using Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;
global using System.Linq;
global using System.Linq.Dynamic.Core;
global using Masa.Mc.Infrastructure.Sms;
global using Masa.Mc.Infrastructure.Sms.Aliyun;
global using Masa.Mc.Contracts.Admin.Options.Channels;
global using Masa.Mc.Infrastructure.Common.Helper;
global using Masa.Mc.Infrastructure.Common.Extensions;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.Sms.Services;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Repositories;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Services;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SmsTemplate;
global using Masa.Mc.Service.Admin.Application.ReceiverGroups.Queries;
global using Masa.Mc.Service.Admin.Application.ReceiverGroups.Commands;
global using Masa.Mc.Contracts.Admin.Enums.ReceiverGroups;
global using System.Collections.ObjectModel;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups.Validator;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Events;
global using Masa.Contrib.Isolation.UoW.EF;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Data.EntityFrameworkCore;
global using Masa.Contrib.Data.EntityFrameworkCore.SqlServer;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Repositories;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Commands;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Queries;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Services;
global using Masa.Mc.Service.Admin.Infrastructure.Repositories;
global using Masa.Mc.Infrastructure.ObjectExtending.ObjectExtending;
global using Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates;
global using Masa.Mc.Service.Admin.Domain.MessageInfos.Repositories;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.Service.Admin.Application.MessageInfos.Queries;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;
global using Masa.Mc.Service.Admin.Application.MessageInfos.Commands;
global using Masa.Contrib.Data.Contracts.EF;
global using Masa.Mc.Infrastructure.Email;
global using Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.Email.Smtp;
global using Masa.Mc.Infrastructure.ObjectExtending.Data;
global using Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;
global using Masa.Mc.Service.Admin.Domain.MessageRecords.Repositories;
global using Masa.Mc.Service.Admin.Domain.MessageRecords.Events;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore.ValueConverters;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Masa.Mc.Contracts.Admin.Dtos.MessageRecords;
global using Masa.Mc.Service.Admin.Application.MessageRecords.Queries;
global using Masa.Mc.Contracts.Admin.Enums.MessageRecords;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SendSms;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Events;
global using Masa.Mc.Contracts.Admin.Dtos;
global using Masa.Mc.Infrastructure.ExporterAndImporter.Csv;
global using ICsvImporter = Masa.Mc.Infrastructure.ExporterAndImporter.Csv.ICsvImporter;
global using System.Dynamic;
global using ICsvExporter = Masa.Mc.Infrastructure.ExporterAndImporter.Csv.ICsvExporter;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.Utils.Extensions.Expressions;
global using Masa.Utils.Caching.DistributedMemory.DependencyInjection;
global using Masa.Utils.Caching.Redis.DependencyInjection;
global using Masa.Utils.Caching.Redis.Models;
global using Masa.Utils.Development.Dapr;
global using Masa.Utils.Development.Dapr.AspNetCore;
global using Dapr;
global using Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates;
global using Masa.Mc.Service.Admin.Domain.WebsiteMessages.Repositories;
global using Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;
global using Microsoft.AspNetCore.SignalR;
global using Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;
global using Masa.Mc.Service.Admin.Application.WebsiteMessages.Queries;
global using Masa.Mc.Service.Admin.Domain.WebsiteMessages.Events;
global using Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR.Hubs;
global using Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR;
global using Microsoft.Extensions.Options;
global using Masa.Mc.Contracts.Admin.Enums.WebsiteMessages;
global using Masa.Mc.Service.Admin.Application.WebsiteMessages.Commands;