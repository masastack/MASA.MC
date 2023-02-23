// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});
builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TLS_NAME")))
        {
            options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN");
        }
        else
        {
            options.ServerCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
        }
        options.CheckCertificateRevocation = false;
    });
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.AddMasaStackComponentsForServer();
var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();
var mcBaseAddress = publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url");
builder.Services.AddMcApiGateways(option => option.McServiceBaseAddress = mcBaseAddress);
builder.Services.AddObservable(builder.Logging, () =>
{
    return new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = "1.0.0",//todo global version
        ServiceName = "masa-mc-web-admin"
    };
}, () =>
{
    return publicConfiguration.GetValue<string>("$public.AppSettings:OtlpUrl");
}, true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddSingleton<ChannelUpsertDtoValidator>();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));
builder.Services.AddMasaOpenIdConnect(publicConfiguration.GetSection("$public.OIDC").Get<MasaOpenIdConnectOptions>());

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();