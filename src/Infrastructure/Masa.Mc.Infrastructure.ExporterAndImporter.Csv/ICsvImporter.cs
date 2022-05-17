using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using System.IO;
using System.Threading.Tasks;

namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv
{
    public interface ICsvImporter : IImporter
    {
        Task<ImportResult<T>> DynamicImport<T>(Stream stream) where T : class, new();
    }
}