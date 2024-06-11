using Autofac.Extensions.DependencyInjection;

namespace WritingPlatformApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new  AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>());
    }
}