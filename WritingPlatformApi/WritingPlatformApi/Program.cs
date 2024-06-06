using Application.Interfaces;
using Application.Services;
using Application.PlatformFeatures;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using CorrelationId.DependencyInjection;
using WritingPlatformApi.Modules;
using Application.PlatformFeatures.Commands;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddHttpLogging(o => o = new HttpLoggingOptions());
builder.Services.AddLogging();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddApplication();

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<IApiClientGoogleDrive, ApiClientGoogleDrive>();
builder.Services.AddScoped<IBlobStorage, BlobStorage>();

builder.Services.AddSingleton<BlobStorageConfig>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Movie Manager API V1");
        c.OAuthAppName("Movie Manager API");
    });
}

//app.UseCorrelationId();

//app.UseMiddleware<HealthCheckMiddleware>();

//app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
//app.UseCors();
app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();