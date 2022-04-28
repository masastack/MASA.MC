namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GenerateImportTemplateQuery : Query<byte[]>
{
    public override byte[] Result { get; set; } = default!;
}
