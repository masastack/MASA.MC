namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

public class CsvExporter : ICsvExporter
{
    public async Task<ExportFileInfo> Export<T>(string fileName, ICollection<T> dataItems) where T : class, new()
    {
        fileName.CheckCsvFileName();
        var bytes = await ExportAsByteArray(dataItems);
        return bytes.ToCsvExportFileInfo(fileName);
    }

    public async Task<ExportFileInfo> Export<T>(string fileName, DataTable dataItems) where T : class, new()
    {
        fileName.CheckCsvFileName();
        var bytes = await ExportAsByteArray<T>(dataItems);
        return bytes.ToCsvExportFileInfo(fileName);
    }

    public Task<byte[]> ExportAsByteArray<T>(ICollection<T> dataItems) where T : class, new()
    {
        var helper = new ExportHelper<T>();
        return Task.FromResult(helper.GetCsvExportAsByteArray(dataItems));
    }

    public Task<byte[]> ExportAsByteArray<T>(DataTable dataItems) where T : class, new()
    {
        var helper = new ExportHelper<T>();
        return Task.FromResult(helper.GetCsvExportAsByteArray(dataItems, null));
    }

    public Task<byte[]> ExportAsByteArray(DataTable dataItems, Type type)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> ExportHeaderAsByteArray<T>(T type) where T : class, new()
    {
        var helper = new ExportHelper<T>();
        return Task.FromResult(helper.GetCsvExportHeaderAsByteArray());
    }

    public Task<byte[]> ExportDynamicHeaderAsByteArray(IDynamicMetaObjectProvider record)
    {
        var helper = new ExportHelper<IDynamicMetaObjectProvider>();
        return Task.FromResult(helper.GetCsvExportDynamicHeaderAsByteArray(record));
    }
}
