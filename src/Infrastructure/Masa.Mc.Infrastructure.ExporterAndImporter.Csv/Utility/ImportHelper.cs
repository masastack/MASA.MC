// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv.Utility;

public class ImportHelper<T> : IDisposable where T : class, new()
{
    public ImportHelper(string filePath = null)
    {
        FilePath = filePath;
    }

    public ImportHelper(Stream stream)
    {
        Stream = stream;
    }

    protected string FilePath { get; set; }

    internal ImportResult<T> ImportResult { get; set; }

    protected Stream Stream { get; set; }

    public Task<ImportResult<T>> Import(string filePath = null)
    {
        if (!string.IsNullOrWhiteSpace(filePath)) FilePath = filePath;

        ImportResult = new ImportResult<T>();
        try
        {
            if (Stream == null)
            {
                CheckImportFile(FilePath);
                Stream = new FileStream(FilePath, FileMode.Open);
            }

            using (var reader = new StreamReader(Stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<AutoMap<T>>();
                var result = csv.GetRecords<T>();
                ImportResult.Data = result.ToList();
                return Task.FromResult(ImportResult);
            }
        }
        catch (Exception ex)
        {
            ImportResult.Exception = ex;
        }
        finally
        {
            ((IDisposable)Stream)?.Dispose();
        }
        return Task.FromResult(ImportResult);
    }

    public Task<ImportResult<dynamic>> DynamicImport(string filePath = null)
    {
        if (!string.IsNullOrWhiteSpace(filePath)) FilePath = filePath;

        var importResult = new ImportResult<dynamic>();
        try
        {
            if (Stream == null)
            {
                CheckImportFile(FilePath);
                Stream = new FileStream(FilePath, FileMode.Open);
            }

            using (var reader = new StreamReader(Stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<AutoMap<T>>();
                var result = csv.GetRecords<dynamic>();
                importResult.Data = result.ToList();
                return Task.FromResult(importResult);
            }
        }
        catch (Exception ex)
        {
            importResult.Exception = ex;
        }
        finally
        {
            ((IDisposable)Stream)?.Dispose();
        }
        return Task.FromResult(importResult);
    }

    public Task<byte[]> GenerateTemplateByte()
    {
        var properties = typeof(T).GetSortedPropertyInfos();

        using (var ms = new MemoryStream())
        using (var writer = new StreamWriter(ms, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.Configuration.HasHeaderRecord = true;
            #region header
            foreach (var prop in properties)
            {
                var name = prop.Name;
                var headerAttribute = prop.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ExporterHeaderAttribute>();
                if (headerAttribute != null)
                {
                    name = headerAttribute.DisplayName ?? prop.GetDisplayName() ?? prop.Name;
                }
                var importAttribute = prop.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ImporterHeaderAttribute>();
                if (importAttribute != null)
                {
                    name = importAttribute.Name ?? prop.GetDisplayName() ?? prop.Name;
                }
                csv.WriteField(name);
            }
            csv.NextRecord();
            #endregion

            writer.Flush();
            ms.Position = 0;
            return Task.FromResult(ms.ToArray());
        }
    }

    private static void CheckImportFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException(Resource.FilePathCannotBeEmpty, nameof(filePath));
    }

    /// <summary>
    /// </summary>
    public void Dispose()
    {
        FilePath = null;
        ImportResult = null;
        Stream = null;
        GC.Collect();
    }
}
