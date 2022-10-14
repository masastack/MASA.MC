// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});
builder.AddObservability();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
var authBaseAddress = builder.Configuration["AuthServiceBaseAddress"];
var mcBaseAddress = builder.Configuration["McServiceBaseAddress"];
builder.Services.AddMcApiGateways(option => option.McServiceBaseAddress = mcBaseAddress);
#if DEBUG
builder.AddMasaStackComponentsForServer("wwwroot/i18n", authBaseAddress, "https://localhost:19501");
#else
builder.AddMasaStackComponentsForServer("wwwroot/i18n", authBaseAddress, mcBaseAddress);
#endif

builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddScoped<TokenProvider>();
//builder.Services.AddElasticsearchClient("auth", option => option.UseNodes("http://10.10.90.44:31920/").UseDefault())
//                .AddAutoComplete(option => option.UseIndexName(builder.Configuration["UserAutoComplete"]));
builder.Services.AddSingleton<ChannelUpsertDtoValidator>();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));
var oidcOptions = builder.Services.GetMasaConfiguration().Local.GetSection("$public.OIDC:AuthClient").Get<MasaOpenIdConnectOptions>();
builder.Services.AddMasaOpenIdConnect(oidcOptions);

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