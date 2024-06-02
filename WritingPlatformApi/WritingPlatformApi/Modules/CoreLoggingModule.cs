namespace WritingPlatformApi.Modules
{
    public static class CoreLoggingModule
    {
        public static IServiceCollection UseCoreLogging(this IServiceCollection services)
        {
            return services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });
        }
    }
}
