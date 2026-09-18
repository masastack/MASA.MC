namespace Masa.Mc.Service.Tests;

public class MessageRecordDetailContentResolverTests
{
    private readonly MessageRecordDetailContentResolver _resolver = new(new TextTemplateRenderer());

    [Fact]
    public void Resolve_ShouldUseMessageInfoForOrdinaryMessage()
    {
        var record = CreateRecord(MessageEntityTypes.Ordinary);
        var messageInfo = new MessageInfoQueryModel { Title = "ordinary", Content = "stored content" };

        var result = _resolver.Resolve(record, messageInfo);

        Assert.Equal(MessageRecordContentSources.MessageInfo, result.Source);
        Assert.True(result.IsReliable);
        Assert.Equal("stored content", result.Content!.Content);
    }

    [Fact]
    public void Resolve_ShouldPreferSnapshotOverCurrentTemplate()
    {
        var record = CreateRecord(MessageEntityTypes.Template);
        record.ContentSnapshot = new MessageRecordContentQueryModel
        {
            Title = "sent",
            Content = "historical content"
        };
        var template = new MessageTemplateQueryModel { Title = "changed", Content = "current content" };

        var result = _resolver.Resolve(record, template: template);

        Assert.Equal(MessageRecordContentSources.RecordSnapshot, result.Source);
        Assert.True(result.IsReliable);
        Assert.Equal("historical content", result.Content!.Content);
    }

    [Fact]
    public void Resolve_ShouldRenderLegacyAliyunTemplateAsUnreliableFallback()
    {
        var record = CreateRecord(MessageEntityTypes.Template);
        record.Channel = new ChannelQueryModel { Type = ChannelTypes.Sms, Provider = (int)SmsProviders.Aliyun };
        record.Variables["name"] = "Alice";
        record.ExtraProperties[nameof(Masa.Mc.Domain.MessageTemplates.Aggregates.MessageTemplate.Sign)] = "MASA";
        var template = new MessageTemplateQueryModel
        {
            Title = "Notice ${name}",
            Content = "Hello ${name}",
            IsJump = true,
            JumpUrl = "/users/${name}"
        };

        var result = _resolver.Resolve(record, template: template);

        Assert.Equal(MessageRecordContentSources.CurrentTemplateFallback, result.Source);
        Assert.False(result.IsReliable);
        Assert.Equal("Notice Alice", result.Content!.Title);
        Assert.Equal("【MASA】Hello Alice", result.Content.Content);
        Assert.Equal("/users/Alice", result.Content.JumpUrl);
    }

    [Theory]
    [InlineData(MessageEntityTypes.Ordinary)]
    [InlineData(MessageEntityTypes.Template)]
    public void Resolve_ShouldReturnUnavailableWhenSourceDoesNotExist(MessageEntityTypes entityType)
    {
        var result = _resolver.Resolve(CreateRecord(entityType));

        Assert.Equal(MessageRecordContentSources.Unavailable, result.Source);
        Assert.False(result.IsReliable);
        Assert.Null(result.Content);
    }

    private static MessageRecordQueryModel CreateRecord(MessageEntityTypes entityType)
    {
        return new MessageRecordQueryModel
        {
            MessageEntityType = entityType,
            Channel = new ChannelQueryModel(),
            Variables = new ExtraPropertyDictionary(),
            ExtraProperties = new ExtraPropertyDictionary()
        };
    }
}
