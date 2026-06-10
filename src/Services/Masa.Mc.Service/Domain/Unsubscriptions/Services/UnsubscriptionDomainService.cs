// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Unsubscriptions.Services;

public class UnsubscriptionDomainService : DomainService
{
    private const string INBOUND_UNSUBSCRIBE_REASON = "用户上行关键字退订";
    private const string INBOUND_RESUBSCRIBE_REASON = "用户上行关键字恢复退订";

    private readonly IUnsubscriptionRepository _repository;

    public UnsubscriptionDomainService(
        IDomainEventBus eventBus,
        IUnsubscriptionRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public async Task<SmsInboundKeywordAction> HandleSmsInboundKeywordAsync(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        int channelProvider,
        Guid templateId,
        SmsInboundKeywordAction action,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        bool debounceEnabled,
        int cooldownSeconds,
        CancellationToken cancellationToken = default)
    {
        if (action == SmsInboundKeywordAction.Unsubscribe)
        {
            var handled = await HandleInboundUnsubscribeAsync(
                userId,
                channelUserIdentity,
                channelId,
                channelProvider,
                templateId,
                keyword,
                occurredAt,
                inboundMessageId,
                debounceEnabled,
                cooldownSeconds,
                cancellationToken);
            return handled ? SmsInboundKeywordAction.Unsubscribe : SmsInboundKeywordAction.None;
        }

        if (action == SmsInboundKeywordAction.Resubscribe)
        {
            var handled = await HandleInboundResubscribeAsync(
                channelId,
                channelUserIdentity,
                templateId,
                keyword,
                occurredAt,
                inboundMessageId,
                cancellationToken);
            return handled ? SmsInboundKeywordAction.Resubscribe : SmsInboundKeywordAction.None;
        }

        return SmsInboundKeywordAction.None;
    }

    private async Task<bool> HandleInboundUnsubscribeAsync(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        int channelProvider,
        Guid templateId,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        bool debounceEnabled,
        int cooldownSeconds,
        CancellationToken cancellationToken = default)
    {
        var scopeRefId = templateId.ToString("N");
        var active = await _repository.FindActiveAsync(
            channelId,
            channelUserIdentity,
            UnsubscriptionScopeTypes.Template,
            scopeRefId,
            cancellationToken);

        if (active is null)
        {
            var aggregate = Unsubscription.CreateFromSmsInbound(
                userId,
                channelUserIdentity,
                channelId,
                channelProvider,
                UnsubscriptionScopeTypes.Template,
                scopeRefId,
                keyword,
                INBOUND_UNSUBSCRIBE_REASON,
                occurredAt,
                inboundMessageId);

            await _repository.AddAsync(aggregate, cancellationToken);
            return true;
        }

        var changed = active.TryUnsubscribeByInboundKeyword(
            keyword,
            INBOUND_UNSUBSCRIBE_REASON,
            occurredAt,
            inboundMessageId,
            debounceEnabled,
            cooldownSeconds);
        if (!changed)
        {
            return false;
        }

        await _repository.UpdateAsync(active, cancellationToken);
        return true;
    }

    private async Task<bool> HandleInboundResubscribeAsync(
        Guid channelId,
        string channelUserIdentity,
        Guid templateId,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        CancellationToken cancellationToken = default)
    {
        var active = await _repository.FindActiveAsync(
            channelId,
            channelUserIdentity,
            UnsubscriptionScopeTypes.Template,
            templateId.ToString("N"),
            cancellationToken);
        if (active is null)
        {
            return false;
        }

        active.ResubscribeByInboundKeyword(
            keyword,
            INBOUND_RESUBSCRIBE_REASON,
            occurredAt,
            inboundMessageId);

        await _repository.UpdateAsync(active, cancellationToken);
        return true;
    }

    public async Task<bool> IsSmsTemplateUnsubscribedAsync(
        Guid channelId,
        string channelUserIdentity,
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        if (channelId == Guid.Empty || templateId == Guid.Empty || string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            return false;
        }

        var aggregate = await _repository.FindActiveAsync(
            channelId,
            channelUserIdentity.Trim(),
            UnsubscriptionScopeTypes.Template,
            templateId.ToString("N"),
            cancellationToken);

        return aggregate is not null;
    }
}
