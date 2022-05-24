// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv.Utility;

public class ExportHelper<T> where T : class
{
    private readonly Type _type;

    public ExportHelper()
    {
    }

    public ExportHelper(Type type)
    {
        _type = type;
    }

    public byte[] GetCsvExportAsByteArray(ICollection<T> dataItems = null, string delimiter = "")
    {
        using (var ms = new MemoryStream())
        using (var writer = new StreamWriter(ms, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.Configuration.HasHeaderRecord = true;

            if (!string.IsNullOrWhiteSpace(delimiter))
                csv.Context.Configuration.Delimiter = delimiter;

            if (_type == null)
            {
                csv.Context.RegisterClassMap<AutoMap<T>>();
            }
            else
            {
                csv.Context.RegisterClassMap<AutoMap<T>>();
            }

            if (dataItems != null && dataItems.Count > 0)
            {
                csv.WriteRecords(dataItems);
            }
            writer.Flush();
            ms.Position = 0;
            return ms.ToArray();
        }
    }

    public byte[] GetCsvExportHeaderAsByteArray(string delimiter = "")
    {
        var properties = typeof(T).GetSortedPropertyInfos();

        using (var ms = new MemoryStream())
        using (var writer = new StreamWriter(ms, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.Configuration.HasHeaderRecord = true;

            if (!string.IsNullOrWhiteSpace(delimiter))
                csv.Context.Configuration.Delimiter = delimiter;

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
            return ms.ToArray();
        }
    }

    public byte[] GetCsvExportDynamicHeaderAsByteArray(IDynamicMetaObjectProvider record, string delimiter = "")
    {
        using (var ms = new MemoryStream())
        using (var writer = new StreamWriter(ms, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.Configuration.HasHeaderRecord = true;

            if (!string.IsNullOrWhiteSpace(delimiter))
                csv.Context.Configuration.Delimiter = delimiter;

            #region header
            csv.WriteDynamicHeader(record);
            csv.NextRecord();
            #endregion

            writer.Flush();
            ms.Position = 0;
            return ms.ToArray();
        }
    }

    public byte[] GetCsvExportAsByteArray(DataTable dataItems, string delimiter = "")
    {
        using (var ms = new MemoryStream())
        using (var writer = new StreamWriter(ms, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<AutoMap<T>>();
            csv.Context.Configuration.HasHeaderRecord = true;

            if (!string.IsNullOrWhiteSpace(delimiter))
                csv.Context.Configuration.Delimiter = delimiter;

            csv.WriteRecords(dataItems.ToList<T>());
            writer.Flush();
            ms.Position = 0;
            return ms.ToArray();
        }
    }
}
