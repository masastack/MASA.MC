// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Tsc;

public static class ObservabilityExtensions
{
    public static void AddObservability(this WebApplicationBuilder builder, bool isBlazor = false)
    {
        var option = builder.Configuration.GetSection("MasaTscOption").Get<MasaObservableOptions>();
        var resources = ResourceBuilder.CreateDefault().AddMasaService(option);
        var opltUrl = builder.Configuration.GetSection("OtlpUrl").Get<string>();
        var uri = new Uri(opltUrl);

        //metrics
        builder.Services.AddMasaMetrics(builder => {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options => options.Endpoint = uri);
        });

        //trcaing
        builder.Services.AddMasaTracing(options =>
        {
            if (isBlazor)
            {
                options.AspNetCoreInstrumentationOptions.AppendDefaultFilter(options);
            }
            else
            {
                options.AspNetCoreInstrumentationOptions.AppendBlazorFilter(options);
            }

            options.BuildTraceCallback = builder =>
            {
                builder.SetResourceBuilder(resources);
                builder.AddOtlpExporter(opt => opt.Endpoint = uri);
            };
        });

        //logging
        builder.Logging.AddMasaOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options => options.Endpoint = uri);
        });
    }
}
