namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

/// <summary>
/// Csv导出程序
/// </summary>
public class CsvExporter : ICsvExporter
{
    /// <summary>
    /// 导出Csv
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="dataItems">数据列</param>
    /// <returns>文件</returns>
    public async Task<ExportFileInfo> Export<T>(string fileName, ICollection<T> dataItems) where T : class, new()
    {
        fileName.CheckCsvFileName();
        var bytes = await ExportAsByteArray(dataItems);
        return bytes.ToCsvExportFileInfo(fileName);
    }
    /// <summary>
    /// 导出Csv
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="dataItems"></param>
    /// <returns></returns>
    public async Task<ExportFileInfo> Export<T>(string fileName, DataTable dataItems) where T : class, new()
    {
        fileName.CheckCsvFileName();
        var bytes = await ExportAsByteArray<T>(dataItems);
        return bytes.ToCsvExportFileInfo(fileName);
    }
    /// <summary>
    /// 导出字节
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataItems">数据列</param>
    /// <returns></returns>
    public Task<byte[]> ExportAsByteArray<T>(ICollection<T> dataItems) where T : class, new()
    {
        var helper = new ExportHelper<T>();
        return Task.FromResult(helper.GetCsvExportAsByteArray(dataItems));
    }
    /// <summary>
    /// 导出DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataItems"></param>
    /// <returns></returns>
    public Task<byte[]> ExportAsByteArray<T>(DataTable dataItems) where T : class, new()
    {
        var helper = new ExportHelper<T>();
        return Task.FromResult(helper.GetCsvExportAsByteArray(dataItems, null));
    }

    /// <summary>
    ///     
    /// </summary>
    /// <param name="dataItems"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<byte[]> ExportAsByteArray(DataTable dataItems, Type type)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 导出Csv表头
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public Task<byte[]> ExportHeaderAsByteArray<T>(T type) where T : class, new()
    {
        var helper = new ExportHelper<T>();
        return Task.FromResult(helper.GetCsvExportHeaderAsByteArray());
    }

    /// <summary>
    /// 导出Csv表头
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public Task<byte[]> ExportDynamicHeaderAsByteArray(IDynamicMetaObjectProvider record)
    {
        var helper = new ExportHelper<IDynamicMetaObjectProvider>();
        return Task.FromResult(helper.GetCsvExportDynamicHeaderAsByteArray(record));
    }
}
