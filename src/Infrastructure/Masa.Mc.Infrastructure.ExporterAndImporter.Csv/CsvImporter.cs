﻿namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

/// <summary>
///     Csv导入
/// </summary>
public class CsvImporter : ICsvImporter
{
    /// <summary>
    /// 导出模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<ExportFileInfo> GenerateTemplate<T>(string fileName) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(fileName))
        {
            return (await importer.GenerateTemplateByte())
                .ToCsvExportFileInfo(fileName);
        }
    }
    /// <summary>
    /// 生成Csv导入模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<byte[]> GenerateTemplateBytes<T>() where T : class, new()
    {
        using (var importer = new ImportHelper<T>())
        {
            return importer.GenerateTemplateByte();
        }
    }

    /// <summary>
    /// 导入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="labelingFilePath"></param>
    /// <returns></returns>
    public Task<ImportResult<T>> Import<T>(string filePath, string labelingFilePath = null, Func<ImportResult<T>, ImportResult<T>> importResultCallback = null) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(filePath))
        {
            return importer.Import();
        }
    }

    /// <summary>
    /// 导入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="importResultCallback"></param>
    /// <returns></returns>
    public Task<ImportResult<T>> Import<T>(string filePath, Func<ImportResult<T>, ImportResult<T>> importResultCallback = null) where T : class, new()
    {
        return Import<T>(filePath, importResultCallback: importResultCallback);
    }


    /// <summary>
    /// 导入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public Task<ImportResult<T>> Import<T>(Stream stream) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(stream))
        {
            return importer.Import();
        }
    }

    /// <summary>
    /// 导入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public Task<ImportResult<dynamic>> DynamicImport<T>(Stream stream) where T : class, new()
    {
        using (var importer = new ImportHelper<T>(stream))
        {
            return importer.DynamicImport();
        }
    }
}
