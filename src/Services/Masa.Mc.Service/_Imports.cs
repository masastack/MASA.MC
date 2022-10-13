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
global using Masa.Contrib.Data.UoW.EFCore;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.Events.Enums;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Masa.Contrib.ReadWriteSplitting.Cqrs.Commands;
global using Masa.Contrib.ReadWriteSplitting.Cqrs.Queries;
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
global using Masa.Contrib.Isolation.UoW.EFCore;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Repositories;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Commands;
global using Masa.Mc.Service.Admin.Application.MessageTasks.Queries;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Services;
global using Masa.Mc.Service.Admin.Infrastructure.Repositories;
global using Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates;
global using Masa.Mc.Service.Admin.Domain.MessageInfos.Repositories;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.Service.Admin.Application.MessageInfos.Queries;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;
global using Masa.Mc.Service.Admin.Application.MessageInfos.Commands;
global using Masa.Contrib.Data.Contracts.EFCore;
global using Masa.Mc.Infrastructure.Email;
global using Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve.Contributors;
global using Masa.Mc.Infrastructure.Email.Smtp;
global using Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;
global using Masa.Mc.Service.Admin.Domain.MessageRecords.Repositories;
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
global using Masa.BuildingBlocks.Caching;
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
global using Microsoft.AspNetCore.Authorization;
global using Masa.Mc.Contracts.Admin.Consts;
global using Masa.Mc.Service.Admin.Domain.WebsiteMessages.Services;
global using Magicodes.ExporterAndImporter.Core.Extension;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun;
global using Dapr.Client;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;
global using Masa.Mc.Contracts.Admin.Dtos.Oss;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.BuildingBlocks.StackSdks.Auth;
global using Masa.Mc.Contracts.Admin.Dtos.Subjects;
global using Masa.Mc.Service.Admin.Application.Subjects.Queries;
global using Masa.Mc.Service.Admin.Application.Subjects.Commands;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
global using Masa.Contrib.StackSdks.Auth;
global using Masa.Mc.Service.Admin.Application.MessageRecords.Commands;
global using Masa.Mc.Service.Admin.Domain.MessageRecords.Events;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;
global using Magicodes.ExporterAndImporter.Core.Models;
global using Masa.Mc.Infrastructure.MailKit;
global using Masa.BuildingBlocks.StackSdks.Scheduler;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Model;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Enum;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Request;
global using Masa.Mc.Service.Admin.Jobs;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Mc.Infrastructure.Common.Utils;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
global using Masa.BuildingBlocks.Configuration;
global using EFCore.BulkExtensions;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.Contrib.StackSdks.Mc.Infrastructure.Extensions;
global using Masa.BuildingBlocks.StackSdks.Mc;
global using Masa.Mc.Infrastructure.Tsc;
global using Masa.BuildingBlocks.Data;
global using System.Net;
global using Masa.Mc.Service.Admin.Infrastructure.Middleware;
global using System.Text.RegularExpressions;
global using Masa.Contrib.StackSdks.Scheduler;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
global using System.Text.Json.Serialization;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Mc.Service.Admin.Infrastructure.Models;
global using System.Security.Principal;
global using System.Xml.Linq;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;