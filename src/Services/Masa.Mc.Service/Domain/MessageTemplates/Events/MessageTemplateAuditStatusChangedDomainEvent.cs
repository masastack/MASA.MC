// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;

public record MessageTemplateAuditStatusChangedToApprovedDomainEvent(Guid TemplateId, string Remarks) : DomainEvent;

public record MessageTemplateAuditStatusChangedToRefuseDomainEvent(Guid TemplateId, string Remarks) : DomainEvent;

