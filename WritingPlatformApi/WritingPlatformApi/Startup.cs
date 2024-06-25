using Persistence.Context;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Services;
using WritingPlatformApi.Modules;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.HttpLogging;
using Application.PlatformFeatures;
using CorrelationId;
using WritingPlatformApi.Middleware;

namespace WritingPlatformApi
{
    public class Startup
    {
        public readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.json", false, true)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCore(_configuration);
            services.AddHttpLogging(o => o = new HttpLoggingOptions());
            services.AddLogging();
            services.AddDefaultCorrelationId();
            services.AddApplication();

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IBlobStorage, BlobStorage>();
            services.AddScoped<IPdfReaderService, PdfReaderPaginatorService>();
            services.AddSingleton<BlobStorageConfig>();

            ConfigureDb(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Writing Platform API V1");
            });

            app.UseCorrelationId();

            app.UseMiddleware<HealthCheckMiddleware>();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpLogging();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void ConfigureDb(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}
