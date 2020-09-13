using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Cli
{
    class BotService : IHostedService
    {
        private readonly Client _client;

        public BotService(Client client)
        {
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.Startup();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
