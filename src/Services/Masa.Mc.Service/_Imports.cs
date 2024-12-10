﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Dynamic;
global using System.Linq.Dynamic.Core;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Dapr;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using FluentValidation.Resources;
global using Magicodes.ExporterAndImporter.Core.Extension;
global using Magicodes.ExporterAndImporter.Core.Models;
global using Mapster;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.Contracts;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Services;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Extensions.BackgroundJobs;
global using Masa.BuildingBlocks.Globalization.I18n;
global using Masa.BuildingBlocks.Isolation;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
global using Masa.BuildingBlocks.StackSdks.Auth;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.BuildingBlocks.StackSdks.Config.Consts;
global using Masa.BuildingBlocks.StackSdks.Config.Models;
global using Masa.BuildingBlocks.StackSdks.Scheduler;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Enum;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Model;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Request;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Masa.Contrib.StackSdks.Caller;
global using Masa.Contrib.StackSdks.Config;
global using Masa.Contrib.StackSdks.Isolation;
global using Masa.Contrib.StackSdks.Middleware;
global using Masa.Contrib.StackSdks.Scheduler;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun;
global using Masa.Mc.Contracts.Admin.Consts;
global using Masa.Mc.Contracts.Admin.Dtos;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.MessageRecords;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.Oss;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.Subjects;
global using Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Contracts.Admin.Enums.MessageRecords;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Contracts.Admin.Enums.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Enums.ReceiverGroups;
global using Masa.Mc.Contracts.Admin.Enums.WebsiteMessages;
global using Masa.Mc.Contracts.Admin.Infrastructure;
global using Masa.Mc.Contracts.Admin.Options.Channels;
global using Masa.Mc.Contracts.Admin.Scheduler;
global using Masa.Mc.EntityFrameworkCore;
global using Masa.Mc.Infrastructure.AppNotification;
global using Masa.Mc.Infrastructure.AppNotification.Infrastructure.OptionsResolve;
global using Masa.Mc.Infrastructure.Cache;
global using Masa.Mc.Infrastructure.Common.Helper;
global using Masa.Mc.Infrastructure.Common.Utils;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Masa.Mc.Infrastructure.Email;
global using Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.Email.Smtp;
global using Masa.Mc.Infrastructure.ExporterAndImporter.Csv;
global using Masa.Mc.Infrastructure.MailKit;
global using Masa.Mc.Infrastructure.OptionsResolve;
global using Masa.Mc.Infrastructure.Sms;
global using Masa.Mc.Infrastructure.Sms.Aliyun;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SendSms;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SmsTemplate;
global using Masa.Mc.Infrastructure.Sms.Services;
global using Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Work;
global using Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.WorkWebhook;
global using Masa.Mc.Infrastructure.Weixin.Work.Model;
global using Masa.Mc.Infrastructure.Weixin.Work.Model.Response;
global using Masa.Mc.Infrastructure.Weixin.Work.Sender;
global using Masa.Mc.Infrastructure.Weixin.Work.Work;
global using Masa.Mc.Infrastructure.Weixin.Work.WorkWebhook;
global using Masa.Mc.Infrastructure.Weixin.Work.Extensions;
global using Masa.Mc.Service.Admin.Application.Channels.Commands;
global using Masa.Mc.Service.Admin.Application.Channels.Queries;
global using Masa.Mc.Service.Admin.Application.MessageInfos.Commands;
global using Masa.Mc.Service.Admin.Application.MessageInfos.Queries;
global using Masa.Mc.Service.Admin.Application.MessageRecords.Commands;
global using Masa.Mc.Service.Admin.Application.MessageRecords.Queries;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Commands;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Queries;
global using Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;
global using Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;
global using Masa.Mc.Service.Admin.Application.QueryContext;
global using Masa.Mc.Service.Admin.Application.QueryModels;
global using Masa.Mc.Service.Admin.Application.ReceiverGroups.Commands;
global using Masa.Mc.Service.Admin.Application.ReceiverGroups.Queries;
global using Masa.Mc.Service.Admin.Application.WebsiteMessages.Commands;
global using Masa.Mc.Service.Admin.Application.WebsiteMessages.Queries;
global using Masa.Mc.Service.Admin.EntityFrameworkCore;
global using Masa.Mc.Service.Admin.Domain.Channels.Services;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Services;
global using Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Services;
global using Masa.Mc.Service.Admin.Domain.WebsiteMessages.Services;
global using Masa.Mc.Service.Admin.Infrastructure.Authentication;
global using Masa.Mc.Service.Admin.Infrastructure.ChannelUserFinder.Provider.Auth;
global using Masa.Mc.Service.Admin.Infrastructure.Extensions;
global using Masa.Mc.Service.Admin.Infrastructure.MessageTaskJobService;
global using Masa.Mc.Service.Admin.Infrastructure.Middleware;
global using Masa.Mc.Service.Admin.Infrastructure.Mock;
global using Masa.Mc.Service.Admin.Infrastructure.Models;
global using Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR;
global using Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR.Hubs;
global using Masa.Mc.Service.Admin.Jobs;
global using Masa.Mc.Service.Infrastructure.Middleware;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi.Models;
global using Microsoft.Extensions.Primitives;
global using Moq;
global using static AlibabaCloud.SDK.Dysmsapi20170525.Models.QuerySmsTemplateListResponseBody;
global using ICsvExporter = Masa.Mc.Infrastructure.ExporterAndImporter.Csv.ICsvExporter;
global using ICsvImporter = Masa.Mc.Infrastructure.ExporterAndImporter.Csv.ICsvImporter;
global using IdentityModel.Client;
global using Masa.Mc.Contracts.Admin.Constants;
global using Masa.Mc.Domain.Channels.Aggregates;
global using Masa.Mc.Domain.Channels.Repositories;
global using Masa.Mc.Domain.MessageTasks.Repositories;
global using Masa.Mc.Domain.MessageTemplates.Events;
global using Masa.Mc.Domain.MessageInfos.Aggregates;
global using Masa.Mc.Domain.MessageInfos.Repositories;
global using Masa.Mc.Domain.MessageRecords.Aggregates;
global using Masa.Mc.Domain.MessageTasks.Aggregates;
global using Masa.Mc.Domain.MessageTasks.Events;
global using Masa.Mc.Domain.MessageTemplates.Aggregates;
global using Masa.Mc.Domain.MessageTemplates.Repositories;
global using Masa.Mc.Domain.MessageRecords.Events;
global using Masa.Mc.Domain.MessageRecords.Repositories;
global using Masa.Mc.Domain.WebsiteMessages.Repositories;
global using Masa.Mc.Domain.ReceiverGroups.Aggregates;
global using Masa.Mc.Domain.ReceiverGroups.Events;
global using Masa.Mc.Domain.ReceiverGroups.Repositories;
global using Masa.Mc.Domain.Channels.Events;
global using Masa.Mc.Domain.WebsiteMessages.Aggregates;
global using Masa.Mc.Domain.WebsiteMessages.Events;
global using Masa.Mc.Domain.Notifications;
global using Masa.Mc.Domain.Consts;
global using Masa.Mc.Data;
global using Masa.Mc.Infrastructure.EntityFrameworkCore.ValueConverters;
global using Masa.Mc.Infrastructure.ObjectExtending;