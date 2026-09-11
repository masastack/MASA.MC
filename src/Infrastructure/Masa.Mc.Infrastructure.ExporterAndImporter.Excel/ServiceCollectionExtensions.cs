namespace Masa.Mc.Infrastructure.ExporterAndImporter.Excel;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddXlsx(this IServiceCollection services)
    {
        services.TryAddSingleton<IXlsxExporter, XlsxExporter>();
        return services;
    }
}
