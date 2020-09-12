using CommandLine;
using HouseholdTaskPlanner.TelegramBot.Repositories;
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
            BotConfiguration botConfiguration = default;
            ApiConfiguration apiConfiguration = default;

            var parser = Parser.Default.ParseArguments<CliOptions>(args)
                .WithParsed(options =>
                {
                    botConfiguration = new BotConfiguration
                    {
                        BotToken = options.BotToken
                    };
                    apiConfiguration = new ApiConfiguration
                    {
                        BackendLocation = options.BackendLocation,
                        BasicAuth = options.BasicAuth ?? string.Empty

                    };
                })
                .WithNotParsed(error =>
                {
                    Console.WriteLine($"{error} could not be parsed.");
                });

            if (botConfiguration == null || apiConfiguration == null)
            {
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