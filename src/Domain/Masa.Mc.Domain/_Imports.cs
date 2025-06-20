// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

// System Namespaces
global using System.Collections.Concurrent;
global using System.Collections.ObjectModel;
global using System.Linq.Expressions;
global using System.Reflection;

// MASA Building Blocks
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.Contracts;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

// MASA MC Domain Aggregates
global using Masa.Mc.Domain.Apps.Aggregates;
global using Masa.Mc.Domain.Channels.Aggregates;
global using Masa.Mc.Domain.MessageInfos.Aggregates;
global using Masa.Mc.Domain.MessageRecords.Aggregates;
global using Masa.Mc.Domain.MessageTasks.Aggregates;
global using Masa.Mc.Domain.MessageTemplates.Aggregates;
global using Masa.Mc.Domain.ReceiverGroups.Aggregates;
global using Masa.Mc.Domain.WebsiteMessages.Aggregates;

// MASA MC Domain Events
global using Masa.Mc.Domain.Channels.Events;
global using Masa.Mc.Domain.MessageRecords.Events;
global using Masa.Mc.Domain.MessageTasks.Events;
global using Masa.Mc.Domain.MessageTemplates.Events;
global using Masa.Mc.Domain.WebsiteMessages.Events;

// MASA MC Infrastructure Helper
global using Masa.Mc.Infrastructure.Common.Helper;

// MASA MC Data
global using Masa.Mc.Data;

global using Masa.Mc.Domain.Shared.Apps;
global using Masa.Mc.Domain.Shared.Channels;
global using Masa.Mc.Domain.Shared.MessageRecords;
global using Masa.Mc.Domain.Shared.MessageTasks;
global using Masa.Mc.Domain.Shared.MessageTemplates;
global using Masa.Mc.Domain.Shared.ReceiverGroups;
global using Masa.Mc.Domain.Shared.Subjects;
global using Masa.Mc.Domain.Shared.WebsiteMessages;

global using Masa.Mc.Domain.Shared.Consts;
global using Masa.Mc.Infrastructure.Common.Utils;

global using Microsoft.AspNetCore.WebUtilities;