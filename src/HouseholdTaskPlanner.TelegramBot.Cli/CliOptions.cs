using CommandLine;

namespace HouseholdTaskPlanner.TelegramBot.Cli
{
    public class CliOptions
    {
        [Option("BotToken", HelpText = "Telegram Bot Token for Api Access")]
        public string BotToken { get; set; }

        [Option("Backend", HelpText = "Where the backend is hosted")]
        public string BackendLocation { get; set; }

        [Option("BasicAuth", HelpText = "Basic Auth for Backend authentification.", Required = false)]
        public string BasicAuth { get; set; }
    }
}