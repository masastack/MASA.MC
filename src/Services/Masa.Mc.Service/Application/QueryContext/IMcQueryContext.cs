// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryContext;

public interface IMcQueryContext
{
    public IQueryable<ChannelQueryModel> ChannelQueryQueries { get; }

    public IQueryable<MessageInfoQueryModel> MessageInfoQueries { get; }

    public IQueryable<MessageRecordQueryModel> MessageRecordQueries { get; }

    public IQueryable<MessageTemplateQueryModel> MessageTemplateQueries { get; }

    public IQueryable<MessageTaskQueryModel> MessageTaskQueries { get; }

    public IQueryable<MessageTaskHistoryQueryModel> MessageTaskHistoryQueries { get; }

    public IQueryable<ReceiverGroupQueryModel> ReceiverGroupQueries { get; }

    public IQueryable<WebsiteMessageQueryModel> WebsiteMessageQueries { get; }

    public IQueryable<WebsiteMessageTagQueryModel> WebsiteMessageTagQueries { get; }

    public IQueryable<MessageReceiverUserQueryModel> MessageReceiverUserQueries { get; }

    public IQueryable<AppDeviceTokenQueryModel> AppDeviceTokenQueries { get; }

    public IQueryable<AppVendorConfigQueryModel> AppVendorConfigQueries { get; }
}