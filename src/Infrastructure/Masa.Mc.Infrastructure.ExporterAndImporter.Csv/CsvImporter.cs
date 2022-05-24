namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

public class CsvImporter : ICsvImporter
{
    public async Task<ExportFileInfo> GenerateTemplate<T>(string fileName) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(fileName))
        {
            return (await importer.GenerateTemplateByte())
                .ToCsvExportFileInfo(fileName);
        }
    }

    public Task<byte[]> GenerateTemplateBytes<T>() where T : class, new()
    {
        using (var importer = new ImportHelper<T>())
        {
            return importer.GenerateTemplateByte();
        }
    }

    public Task<ImportResult<T>> Import<T>(string filePath, string labelingFilePath = null, Func<ImportResult<T>, ImportResult<T>> importResultCallback = null) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(filePath))
        {
            return importer.Import();
        }
    }

    public Task<ImportResult<T>> Import<T>(string filePath, Func<ImportResult<T>, ImportResult<T>> importResultCallback = null) where T : class, new()
    {
        return Import<T>(filePath, importResultCallback: importResultCallback);
    }

    public Task<ImportResult<T>> Import<T>(Stream stream) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(stream))
        {
            return importer.Import();
        }
    }

    public Task<ImportResult<dynamic>> DynamicImport<T>(Stream stream) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(stream))
        {
            return importer.DynamicImport();
        }
    }
}
