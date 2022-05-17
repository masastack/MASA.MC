using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv
{
    public interface ICsvExporter : IExporter
    {
        Task<byte[]> ExportDynamicHeaderAsByteArray(IDynamicMetaObjectProvider record);
    }
}