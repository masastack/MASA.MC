namespace Masa.Mc.Infrastructure.ExporterAndImporter.Excel;

public sealed class XlsxExporter : IXlsxExporter
{
    public Task<byte[]> ExportAsByteArray<T>(ICollection<T> dataItems) where T : class, new()
    {
        var exporter = new Magicodes.ExporterAndImporter.Excel.ExcelExporter();
        return exporter.ExportAsByteArray(dataItems);
    }
}
