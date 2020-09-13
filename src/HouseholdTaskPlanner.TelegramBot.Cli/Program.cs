using HouseholdTaskPlanner.TelegramBot.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Cli
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .RunConsoleAsync();
        }

        private static void ConfigureLogging(HostBuilderContext ctx, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            loggingBuilder.AddSerilog(Log.Logger);
        }

        private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.Configure<BotConfiguration>(ctx.Configuration);
            services.Configure<ApiConfiguration>(ctx.Configuration);

            services.AddSingleton<Client>();
            services.AddSingleton<IUserRemoteRepository, UserRemoteRepository>();
            services.AddSingleton<IRecurringTaskRemoteRepository, RecurringTaskRemoteRepository>();
            services.AddSingleton<IScheduledTaskRemoteRepository, ScheduledTaskRemoteRepository>();
            services.AddSingleton<TaskplannerApiHttpClientFactory>();

            services.AddHostedService<BotService>();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext ctx, IConfigurationBuilder configBuilder)
        {
            configBuilder
                .AddEnvironmentVariables()
                .Build();
        }
    }
}