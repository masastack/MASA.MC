namespace Masa.Mc.Service.Tests;

public class MessageRecordTests
{
    [Fact]
    public void CaptureTemplateContent_ShouldRejectOrdinaryMessage()
    {
        var record = CreateRecord(MessageEntityTypes.Ordinary);

        Assert.Throws<InvalidOperationException>(() => record.CaptureTemplateContent(CreateContent("ordinary")));
        Assert.Null(record.ContentSnapshot);
    }

    [Fact]
    public void CaptureTemplateContent_ShouldDeepCopyContentAndExtraProperties()
    {
        var nested = new Dictionary<string, string> { ["label"] = "before" };
        var source = CreateContent("before", new ExtraPropertyDictionary { ["button"] = nested });
        var record = CreateRecord(MessageEntityTypes.Template);

        record.CaptureTemplateContent(source);
        nested["label"] = "after";
        source.ExtraProperties["new"] = "changed";

        Assert.NotSame(source, record.ContentSnapshot);
        Assert.NotSame(source.ExtraProperties, record.ContentSnapshot!.ExtraProperties);
        Assert.False(record.ContentSnapshot.ExtraProperties.ContainsKey("new"));
        var button = Assert.IsType<JsonElement>(record.ContentSnapshot.ExtraProperties["button"]);
        Assert.Equal("before", button.GetProperty("label").GetString());
    }

    [Fact]
    public void RefreshTemplateContentForRetry_ShouldReplaceSnapshotAfterFailure()
    {
        var record = CreateRecord(MessageEntityTypes.Template);
        record.CaptureTemplateContent(CreateContent("first"));
        record.SetResult(false, "provider failure");

        record.RefreshTemplateContentForRetry(CreateContent("retry"));

        Assert.Equal("retry", record.ContentSnapshot!.Content);
    }

    [Fact]
    public void RefreshTemplateContentForRetry_ShouldRejectSuccessfulRecord()
    {
        var record = CreateRecord(MessageEntityTypes.Template);
        record.CaptureTemplateContent(CreateContent("sent"));
        record.SetResult(true, string.Empty);

        Assert.Throws<InvalidOperationException>(() =>
            record.RefreshTemplateContentForRetry(CreateContent("retry")));
        Assert.Equal("sent", record.ContentSnapshot!.Content);
    }

    private static MessageRecord CreateRecord(MessageEntityTypes type)
    {
        var record = new MessageRecord(
            Guid.NewGuid(),
            "receiver",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new ExtraPropertyDictionary(),
            "display",
            DateTimeOffset.UtcNow,
            "system");
        record.SetMessageEntity(type, Guid.NewGuid());
        return record;
    }

    private static MessageContent CreateContent(
        string content,
        ExtraPropertyDictionary? extraProperties = null)
    {
        return new MessageContent("title", content, "markdown", true, "/jump", extraProperties ?? new());
    }
}
