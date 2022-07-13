// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});
builder.Services.AddAuthApiGateways(option => option.McServiceBaseAddress = builder.Configuration["McServiceBaseAddress"]);
builder.Services.AddMasaStackComponentsForServer("wwwroot/i18n", builder.Configuration["AuthServiceBaseAddress"], builder.Configuration["McServiceBaseAddress"]);
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddElasticsearchClient("auth", option => option.UseNodes("http://10.10.90.44:31920/").UseDefault())
                .AddAutoComplete(option => option.UseIndexName("user_index"));
builder.Services.AddSingleton<ChannelUpsertDtoValidator>();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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