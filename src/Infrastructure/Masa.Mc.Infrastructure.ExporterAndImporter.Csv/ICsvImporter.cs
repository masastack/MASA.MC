namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

public interface ICsvImporter : IImporter
{
    Task<ImportResult<dynamic>> DynamicImport<T>(Stream stream) where T : class, new();
}
