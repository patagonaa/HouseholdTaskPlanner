using HouseholdTaskPlanner.Common.Db;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.WasteCollectionImporter
{
    class Program
    {
        // http://www.landkreis-heilbronn.de/online-abfallkalender.7005.htm

        static async Task Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("missing argument file");
                return;
            }

            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("./config/appsettings.json")
                    .Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IScheduledTaskRepository, ScheduledTaskRepository>();
            serviceCollection.Configure<DbConfiguration>(configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var setOutString = configuration.GetValue<string>("SetOutString");
            var retrieveString = configuration.GetValue<string>("RetrieveString");

            var scheduledTaskRepository = serviceProvider.GetRequiredService<IScheduledTaskRepository>();

            var file = new StreamReader(args[0], Encoding.GetEncoding("iso-8859-1"));

            var headerLine = file.ReadLine().Split(';');
            while (!file.EndOfStream)
            {
                var line = file.ReadLine().Split(';');
                for (int i = 0; i < line.Length; i++)
                {
                    var date = line[i];
                    if (string.IsNullOrEmpty(date) || !DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsedDate))
                        continue;
                    var name = headerLine[i].Split(',')[0];

                    await scheduledTaskRepository.Insert(new ScheduledTask
                    {
                        Date = parsedDate.AddDays(-1),
                        Name = string.Format(setOutString, name),
                        State = DateTime.Now > parsedDate ? ScheduledTaskState.Done : ScheduledTaskState.Todo
                    });
                    await scheduledTaskRepository.Insert(new ScheduledTask
                    {
                        Date = parsedDate,
                        Name = string.Format(retrieveString, name),
                        State = DateTime.Now > parsedDate ? ScheduledTaskState.Done : ScheduledTaskState.Todo
                    });
                }
            }
        }
    }
}
