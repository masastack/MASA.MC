// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using FluentValidation;
global using Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;
global using Masa.Mc.Contracts.Admin.Enums.Channels;
global using Masa.Mc.Contracts.Admin.Enums.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Dtos.Channels;
global using Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;
global using Masa.Mc.Contracts.Admin.Enums.ReceiverGroups;
global using Masa.Mc.Infrastructure.Common.Extensions;
global using Masa.Mc.Contracts.Admin.Enums.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos.MessageTasks;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;
global using Masa.Mc.Contracts.Admin.Dtos.MessageInfos;
global using Masa.Mc.Contracts.Admin.Enums.MessageRecords;
global using Masa.Mc.Contracts.Admin.Enums.Subjects;
global using Masa.Mc.Infrastructure.Common.Utils;
global using Masa.Mc.Contracts.Admin.Enums.WebsiteMessages;
global using Masa.Mc.Contracts.Admin.Scheduler;
global using Magicodes.ExporterAndImporter.Core;
global using Magicodes.ExporterAndImporter.Core.Models;
global using Microsoft.AspNetCore.Http;
global using System.Reflection;
global using System.Collections.ObjectModel;
global using System.ComponentModel.DataAnnotations;
global using System.Collections.Concurrent;
global using IdentityModel.Client;
global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.Contrib.StackSdks.Caller;
global using Masa.Contrib.StackSdks.Config;
global using Microsoft.Extensions.Primitives;
global using ExtraPropertyDictionary = Masa.Mc.Data.ExtraPropertyDictionary;