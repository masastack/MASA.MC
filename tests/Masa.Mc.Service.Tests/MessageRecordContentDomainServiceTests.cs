namespace Masa.Mc.Service.Tests;

public class MessageRecordContentDomainServiceTests
{
    [Fact]
    public void CreateSms_ShouldRenderMappedAliyunVariablesAndPresentationRules()
    {
        var content = new MessageContent(
            "Notice ${name}",
            "Hello ${name}",
            string.Empty,
            true,
            "/users/${name}",
            new ExtraPropertyDictionary { ["layout"] = "compact" });
        var template = new MessageTemplate(
            Guid.NewGuid(),
            "sms",
            "sms-code",
            content,
            string.Empty,
            "provider-template",
            "MASA",
            (int)SmsTemplateTypes.Notification,
            0,
            new MessageTemplateUnsubscribeConfig(true, "TD", Guid.NewGuid(), "HF", Guid.NewGuid()));
        template.AddOrUpdateItem("userName", "name", "name", string.Empty);
        var variables = new ExtraPropertyDictionary { ["userName"] = "Alice" };

        var snapshot = CreateService().CreateSms(template, variables, SmsProviders.Aliyun, "MASA");

        Assert.Equal("Notice Alice", snapshot.Title);
        Assert.Equal($"【MASA】Hello Alice{Environment.NewLine}【回复TD退订】", snapshot.Content);
        Assert.Equal("/users/Alice", snapshot.JumpUrl);
        Assert.True(snapshot.IsJump);
        Assert.Equal("compact", snapshot.ExtraProperties["layout"].ToString());
    }

    private static MessageRecordContentDomainService CreateService()
    {
        var eventBus = Mock.Of<IDomainEventBus>();
        return new MessageRecordContentDomainService(eventBus, new TextTemplateRenderer());
    }
}
