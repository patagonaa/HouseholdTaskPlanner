using HouseholdTaskPlanner.TelegramBot.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Cli
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();

            var configuration = configurationBuilder
                                    .AddEnvironmentVariables()
                                    .Build();

            BotConfiguration botConfiguration = new BotConfiguration
            {
                BotToken = configuration[KnownEnvironmentVariables.BotToken]
            };
            ApiConfiguration apiConfiguration = new ApiConfiguration
            {
                BackendLocation = configuration[KnownEnvironmentVariables.Backend],
                BasicAuth = configuration[KnownEnvironmentVariables.BasicAuth]
            };

            if (string.IsNullOrWhiteSpace(botConfiguration.BotToken) ||
                string.IsNullOrWhiteSpace(apiConfiguration.BackendLocation))
            {
                Console.Error.WriteLine("Invalid configuration. BotToken and Backend are required parameters");
                return;
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiConfiguration.BackendLocation),
            };

            if (!string.IsNullOrWhiteSpace(apiConfiguration.BasicAuth))
            {
                var authToken = Encoding.ASCII.GetBytes(apiConfiguration.BasicAuth);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(authToken));
            }

            var userRepo = new UserRemoteRepository(httpClient);
            var recurringRepo = new RecurringTaskRemoteRepository(httpClient);
            var scheduledRepo = new ScheduledTaskRemoteRepository(httpClient);

            var client = new Client(botConfiguration, userRepo, recurringRepo, scheduledRepo);
            await client.Startup();
        }
    }
}