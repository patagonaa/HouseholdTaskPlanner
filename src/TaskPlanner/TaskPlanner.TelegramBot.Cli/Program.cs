using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;
using TaskPlanner.Common.Api;
using User.Common.Api;

namespace TaskPlanner.TelegramBot.Cli
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
            services.Configure<TaskApiConfiguration>(ctx.Configuration.GetSection("TaskApi"));
            services.Configure<UserApiConfiguration>(ctx.Configuration.GetSection("UserApi"));

            services.AddSingleton<Client>();
            services.AddSingleton<IUserRemoteRepository, UserRemoteRepository>();
            services.AddSingleton<IRecurringTaskRemoteRepository, RecurringTaskRemoteRepository>();
            services.AddSingleton<IScheduledTaskRemoteRepository, ScheduledTaskRemoteRepository>();

            services.AddHostedService<BotService>();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext ctx, IConfigurationBuilder configBuilder)
        {
            configBuilder
                .AddJsonFile("configs/appSettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}