// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
global using Masa.Mc.Contracts.Admin.Consts;
global using Masa.Mc.Contracts.Admin.Enums.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Contracts.Admin.Enums.ReceiverGroups;
global using Masa.Mc.Domain.Channels.Aggregates;
global using Masa.Mc.Domain.Channels.Events;
global using Masa.Mc.Domain.MessageInfos.Aggregates;
global using ExtraPropertyDictionary = Masa.Mc.Data.ExtraPropertyDictionary;
global using Masa.Mc.Domain.MessageRecords.Aggregates;
global using Masa.Mc.Domain.MessageRecords.Events;
global using Masa.Mc.Domain.MessageTasks.Aggregates;
global using Masa.Mc.Domain.MessageTasks.Events;
global using Masa.Mc.Domain.MessageTemplates.Aggregates;
global using Masa.Mc.Domain.MessageTemplates.Events;
global using Masa.Mc.Domain.ReceiverGroups.Aggregates;
global using Masa.Mc.Domain.WebsiteMessages.Aggregates;
global using Masa.Mc.Domain.WebsiteMessages.Events;
global using Masa.Mc.Infrastructure.Common.Helper;
global using System.Collections.Concurrent;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq.Expressions;
global using System.Reflection;
global using Masa.Mc.Data;