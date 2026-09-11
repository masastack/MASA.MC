namespace Masa.Mc.Infrastructure.ExporterAndImporter.Excel;

public interface IXlsxExporter
{
    Task<byte[]> ExportAsByteArray<T>(ICollection<T> dataItems) where T : class, new();
}
