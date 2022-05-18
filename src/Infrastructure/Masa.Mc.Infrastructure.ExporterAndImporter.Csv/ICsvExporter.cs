namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

public interface ICsvExporter : IExporter
{
    Task<byte[]> ExportDynamicHeaderAsByteArray(IDynamicMetaObjectProvider record);
}
