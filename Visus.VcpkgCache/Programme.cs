// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Visus.HeaderAuthentication;
using Visus.HeaderAuthentication.Handlers;
using Visus.VcpkgCache;


var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel.
builder.WebHost.ConfigureKestrel(o => {
    builder.Configuration.GetSection("KestrelLimits").Bind(o.Limits);
});

// Add services to the container.
builder.Services.AddSingleton<IValidateOptions<Settings>,
    FluentValidateOptions<Settings, SettingsValidator>>();

builder.Services.AddOptionsWithValidateOnStart<Settings>().Configure(o => {
    builder.Configuration.GetSection(Settings.Section).Bind(o);
});

builder.Services.AddHeaderAuthentication("Token", o => {
    var settings = new Settings();
    builder.Configuration.GetSection(Settings.Section).Bind(settings);
    o.HeaderHandler = new TokenHeaderHandler("Token", settings.Token);
});

builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//if (app.Environment.IsDevelopment()) {
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.Run();
