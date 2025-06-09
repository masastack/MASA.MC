// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

// System Namespaces
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text.Json;

// Microsoft Namespaces
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// Third-party Libraries
global using EFCore.BulkExtensions;

// MASA Building Blocks and Contrib Packages
global using Masa.BuildingBlocks.Data.Contracts;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;

// MASA MC Domain Aggregates
global using Masa.Mc.Domain.Apps.Aggregates;
global using Masa.Mc.Domain.Apps.Repositories;
global using Masa.Mc.Domain.Channels.Aggregates;
global using Masa.Mc.Domain.MessageInfos.Aggregates;
global using Masa.Mc.Domain.MessageRecords.Aggregates;
global using Masa.Mc.Domain.MessageTasks.Aggregates;
global using Masa.Mc.Domain.MessageTemplates.Aggregates;
global using Masa.Mc.Domain.ReceiverGroups.Aggregates;
global using Masa.Mc.Domain.WebsiteMessages.Aggregates;

// MASA MC Domain Repositories
global using Masa.Mc.Domain.Channels.Repositories;
global using Masa.Mc.Domain.MessageInfos.Repositories;
global using Masa.Mc.Domain.MessageRecords.Repositories;
global using Masa.Mc.Domain.MessageTasks.Repositories;
global using Masa.Mc.Domain.MessageTemplates.Repositories;
global using Masa.Mc.Domain.ReceiverGroups.Repositories;
global using Masa.Mc.Domain.WebsiteMessages.Repositories;

// MASA MC Contracts and Consts
global using Masa.Mc.Domain.Consts;

// MASA MC Entity Framework Core
global using Masa.Mc.EntityFrameworkCore;
global using Masa.Mc.EntityFrameworkCore.Repositories;
global using Masa.Mc.EntityFrameworkCore.ValueConverters;
global using Masa.Mc.Infrastructure.EntityFrameworkCore.ValueConverters;
global using Masa.Mc.Domain.Shared.MessageTasks;