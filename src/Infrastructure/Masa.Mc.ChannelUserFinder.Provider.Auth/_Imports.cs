// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Mc.Service.Admin.Domain.MessageTasks.Services;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;
global using System.Collections.Concurrent;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Service.Admin.Domain.MessageTasks.Events;
global using Masa.BuildingBlocks.StackSdks.Auth;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Contracts.Admin.Enums.ReceiverGroups;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;
global using Masa.Mc.Service.Admin.Domain.ReceiverGroups.Repositories;
global using Masa.Mc.Service.Admin.Migrations;
global using MessageReceiverUser = Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageReceiverUser;
global using Receiver = Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates.Receiver;