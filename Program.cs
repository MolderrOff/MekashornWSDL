using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using Mekashron;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceReference1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICUTechClient>(sp =>
{
    var client = new ICUTechClient();

    client.ClientCredentials.UserName.UserName = "myName";
    client.ClientCredentials.UserName.Password = "http_password";
    return client;
});

builder.Services.AddScoped<SoapService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    var address = app.Urls.FirstOrDefault(u => u.StartsWith("http://")) ?? app.Urls.FirstOrDefault();

    if (address != null)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = $"{address}/index.html",
            UseShellExecute = true
        });
    }
});
app.Run();

