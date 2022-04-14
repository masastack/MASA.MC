﻿global using System.Text.Json;
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
global using Masa.Utils.Data.EntityFrameworkCore;
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
global using Masa.Mc.Service.Admin.Infrastructure.Extensions;
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
global using Masa.Mc.Service.Admin.Domain.Channels.Events;
global using System.Linq;
global using System.Linq.Dynamic.Core;
global using Masa.Mc.Infrastructure.Sms;
global using Masa.Mc.Infrastructure.Sms.Aliyun;
global using Masa.Mc.Contracts.Admin.Options.Channels;
global using Masa.Mc.Infrastructure.Common.Helper;
global using Masa.Mc.Infrastructure.Sms.Aliyun.Response.SmsTemplate;