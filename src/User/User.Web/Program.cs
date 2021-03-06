using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HouseholdPlanner.User
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                    config
                    .AddEnvironmentVariables()
                    .AddJsonFile("./config/appsettings.json", optional: true))
                .ConfigureLogging(ConfigureLogging)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }

        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            loggingBuilder.AddSerilog(Log.Logger);
        }
    }
}
