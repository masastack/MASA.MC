// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
GlobalValidationOptions.SetDefaultCulture("zh-CN");

await builder.Services.AddMasaStackConfigAsync();
var masaStackConfig = builder.Services.GetMasaStackConfig();

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    {
        if (string.IsNullOrEmpty(masaStackConfig.TlsName))
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
builder.Services.AddObservable(builder.Logging, () =>
{
    return new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = masaStackConfig.Version,
        ServiceName = masaStackConfig.GetWebId(MasaStackConstant.MC)
    };
}, () =>
{
    return masaStackConfig.OtlpUrl;
}, true);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
var authBaseAddress = masaStackConfig.GetAuthServiceDomain();
var mcBaseAddress = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault().GetValue<string>("AppSettings:McClient:Url");

if (string.IsNullOrEmpty(mcBaseAddress))
{
    mcBaseAddress = masaStackConfig.GetMcServiceDomain();
}

builder.Services.AddMcApiGateways(option => option.McServiceBaseAddress = mcBaseAddress);
builder.AddMasaStackComponentsForServer("wwwroot/i18n", authBaseAddress, mcBaseAddress);

builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddSingleton<ChannelUpsertDtoValidator>();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));
MasaOpenIdConnectOptions masaOpenIdConnectOptions = new MasaOpenIdConnectOptions
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackConstant.MC),
    Scopes = new List<string> { "offline_access" }
}; ;

IdentityModelEventSource.ShowPII = true;
builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions);

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