// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCsv(this IServiceCollection services)
    {
        services.TryAddSingleton<ICsvExporter, CsvExporter>();
        services.TryAddSingleton<ICsvImporter, CsvImporter>();
        return services;
    }
}
