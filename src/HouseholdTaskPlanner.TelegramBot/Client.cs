using HouseholdTaskPlanner.TelegramBot.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HouseholdTaskPlanner.TelegramBot
{
    public class Client
    {
        private readonly TelegramBotClient _bot;
        private readonly BotConfiguration _configuration;
        private readonly IScheduledTaskRemoteRepository _scheduledRepository;
        private readonly IUserRemoteRepository _userRepository;
        private readonly IRecurringTaskRemoteRepository _recurringTaskRepository;
        private readonly ILogger<Client> _logger;
        private CronlikeTimer _dailyScheduler;
        private CronlikeTimer _weeklyScheduler;

        public Client(IOptions<BotConfiguration> configuration,
                      ILogger<Client> logger,
                      IUserRemoteRepository userRepository,
                      IRecurringTaskRemoteRepository recurringTaskRepository,
                      IScheduledTaskRemoteRepository scheduledRepository)
        {
            _configuration = configuration.Value;
            _bot = new TelegramBotClient(_configuration.BotToken);
            _scheduledRepository = scheduledRepository;
            _userRepository = userRepository;
            _recurringTaskRepository = recurringTaskRepository;
            _logger = logger;
        }

        public async Task Start()
        {
            var me = await _bot.GetMeAsync();
            Console.Title = me.Username;

            _dailyScheduler = new CronlikeTimer("0 8 * * 1-6", () => SendDailyTasks().Start());
            _weeklyScheduler = new CronlikeTimer("0 8 * * 7", () => SendWeeklyTasks().Start());

            _dailyScheduler.Start();
            _weeklyScheduler.Start();

            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _bot.OnReceiveError += BotOnReceiveError;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
        }

        public Task Stop()
        {
            _bot.StopReceiving();
            return Task.CompletedTask;
        }


        private async Task SendDailyTasks()
        {
            try
            {
                await ListUpcomingTasks(
                new Message
                {
                    Chat = new Chat { Id = _configuration.ScheduleChat }
                },
                lookahead: TimeSpan.FromDays(1));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not send daily tasks");
            }
        }

        private async Task SendWeeklyTasks()
        {
            try
            {
                await ListUpcomingTasks(
                new Message
                {
                    Chat = new Chat
                    {
                        Id = _configuration.ScheduleChat
                    }
                },
                lookahead: TimeSpan.FromDays(7));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not send weekly tasks");
            }
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            _logger.LogInformation("{ChatId} {UserId} {messageText}", messageEventArgs.Message.Chat.Id, messageEventArgs.Message.From.Id, messageEventArgs.Message.Text);

            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (!_configuration.AllowedChats.Contains(message.Chat.Id))
            {
                await SendNotAllowedChatMessage(message.Chat.Id);
                return;
            }

            switch (GetMessageCommand(message.Text))
            {
                // create new task
                case "/createRecurring":
                    await CreateNewRecurringTask(message);
                    break;

                // create new task
                case "/createSingle":
                    await CreateNewScheduledTask(message);
                    break;

                // list all recurring tasks
                case "/listSeries":
                    await ListRecurringTasks(message);
                    break;

                // list all scheduled tasks (debug functionality, might be removed)
                case "/listAll":
                    await ListUpcomingTasks(message);
                    break;

                // list upcoming tasks this week
                case "/listToday":
                    await ListUpcomingTasks(message, TimeSpan.FromDays(1));
                    break;

                // Send inline keyboard
                case "/listWeek":
                    await ListUpcomingTasks(message, TimeSpan.FromDays(7));
                    break;

                default:
                    await Usage(message);
                    break;
            }
        }

        private async Task<string> GetMessageTextForScheduledMessage(int scheduledTaskId)
        {
            var scheduledTask = await _scheduledRepository.Get(scheduledTaskId);
            string description = scheduledTask.Description;

            var users = (await _userRepository.GetAll()).ToList();

            var assignedUserId = scheduledTask.AssignedUser;
            var assignedUser = assignedUserId.HasValue ? users.Find(user => user.Id == assignedUserId.GetValueOrDefault(-1)) : null;

            return string.Join(Environment.NewLine,
                               $"{scheduledTask.Name} | {(assignedUser != null ? $"Assigned to @{assignedUser.TelegramUsername}" : "Not Assigned")}",
                               string.Empty,
                               description).Trim();
        }

        private async Task ListUpcomingTasks(Message message, TimeSpan? lookahead = default)
        {
            var today = DateTime.Today;
            var scheduledTasks = (await _scheduledRepository.GetTodoList())
                    .Where(task => task.Date >= today).ToList();

            if (lookahead.HasValue)
            {
                var lookaheadDate = today + lookahead.Value;
                scheduledTasks = scheduledTasks.Where(task => task.Date <= lookaheadDate).ToList();
            }

            var users = await _userRepository.GetAll();

            if (scheduledTasks.Count == 0)
            {
                await this.SendKeyboard(message, "No scheduled tasks available", default);
            }

            foreach (var task in scheduledTasks)
            {
                var assignedUserId = task.AssignedUser;
                var assignedUser = assignedUserId.HasValue ? users.SingleOrDefault(user => user.Id == assignedUserId.GetValueOrDefault(-1)) : null;

                string messageText = string.Join(Environment.NewLine,
                                    $"{((lookahead ?? TimeSpan.MaxValue) > TimeSpan.FromDays(1) ? (task.Date.ToString("dddd yyyy-MM-dd", CultureInfo.InvariantCulture) + " | ") : string.Empty)}{task.Name} | {(assignedUser != null ? $"Assigned to @{assignedUser.TelegramUsername}" : "Not Assigned")}",
                                    string.Empty,
                                    task.Description);
                if (assignedUser == null)
                {
                    await this.SendKeyboard(message, messageText, ScheduledMessageDefaultKeyboard(task.Id));
                }
                else
                {
                    await this.SendKeyboard(message, messageText, ScheduledMessageAssignedKeyboard(task.Id));
                }
            }
        }

        private async Task ListRecurringTasks(Message message)
        {
            var recurringTasks = await _recurringTaskRepository.GetAll();

            var taskList = new List<Task>();

            if (recurringTasks.Count == 0)
            {
                taskList.Add(this.SendKeyboard(message, "No recurring tasks available", default));
            }

            foreach (var task in recurringTasks)
            {
                string messageText = string.Join(Environment.NewLine,
                                    $"{task.Name} | Interval {task.IntervalDays} days",
                                    string.Empty,
                                    task.Description);
                taskList.Add(this.SendKeyboard(message, messageText, RecurringMessageDefaultKeyboard(task.Id)));
            }

            await Task.WhenAll(taskList);
        }

        private async Task Usage(Message message)
        {
            string usage = string.Join(Environment.NewLine,
                                   "Usage:",
                                   "/createSingle - create a one-off task",
                                   "/createRecurring - create a new task",
                                   "/listToday - list upcoming tasks today",
                                   "/listWeek - list upcoming tasks this week",
                                   "/listSeries - list all recurring tasks",
                                   "/listAll - list all upcoming tasks"
                                   );
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task CreateNewRecurringTask(Message message)
        {
            var contentLines = GetMessageContent(message.Text).Split('\n');

            string daysStr = contentLines.Length >= 2 ? contentLines[0] : string.Empty;
            bool canParseDays = int.TryParse(daysStr, out int parsedDays);

            if (contentLines.Length < 2 || !canParseDays)
            {
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Use this command to create a new recurring task. Use following structure to do so. Scheduled time is in days."
                );
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "/createRecurring 7\nVacuum Weekly\nTo ensure a clean space for living, it is adviseable to vacuum at least once a week commonly used rooms."
                );
                return;
            }

            await SendKeyboard(message,
                               SerializeHumanReadable(new string[] { daysStr, contentLines[1], string.Join(Environment.NewLine, contentLines.Skip(2))}),
                               RecurringMessageAcceptKeyboard(), false
                               );
        }

        private async Task CreateNewScheduledTask(Message message)
        {
            var contentLines = GetMessageContent(message.Text).Split('\n');
            string daysStr = contentLines.Length >= 2 ? contentLines[0] : string.Empty;
            bool canParseDays = int.TryParse(daysStr, out int parsedDays);

            if (contentLines.Length != 2 || !canParseDays)
            {
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Use this command to schedule a one-off task. Use following structure to do so. Scheduled time is in days."
                );
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "/createSingle 4 (days)\nTake special garbage out"
                );
                return;
            }

            await SendKeyboard(message,
                               SerializeHumanReadable(new string[] { daysStr, contentLines[1] }),
                               ScheduledMessageAcceptKeyboard(), false
                               );
        }

        private InlineKeyboardMarkup ScheduledMessageDefaultKeyboard(int taskId)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("✔ I can do that!", GetCallbackData(CallbackType.ScheduledTask, ActionType.Assign, taskId)),
                    InlineKeyboardButton.WithCallbackData("🚮 Delete ", GetCallbackData(CallbackType.ScheduledTask, ActionType.Delete, taskId))
                }
            });

        private InlineKeyboardMarkup ScheduledMessageAssignedKeyboard(int taskId)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("❌ I can't do that!", GetCallbackData(CallbackType.ScheduledTask, ActionType.Unassign, taskId)),
                    InlineKeyboardButton.WithCallbackData("✔ I'm done!", GetCallbackData(CallbackType.ScheduledTask, ActionType.Done, taskId)),
                }
            });

        private InlineKeyboardMarkup ScheduledMessageAcceptKeyboard()
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("❌ Not what I wanted", GetCallbackData(CallbackType.ScheduledTask, ActionType.Dismiss)),
                    InlineKeyboardButton.WithCallbackData("✔ Schedule this task", GetCallbackData(CallbackType.ScheduledTask, ActionType.Accept))
                }
            });

        private InlineKeyboardMarkup RecurringMessageAcceptKeyboard()
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("❌ Not what I wanted", GetCallbackData(CallbackType.RecurringTask, ActionType.Dismiss)),
                    InlineKeyboardButton.WithCallbackData("✔ Schedule this task", GetCallbackData(CallbackType.RecurringTask, ActionType.Accept))
                }
            });

        private InlineKeyboardMarkup RecurringMessageDefaultKeyboard(int taskId)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("✏ Edit ", GetCallbackData(CallbackType.RecurringTask, ActionType.Edit, taskId)),
                    InlineKeyboardButton.WithCallbackData("🚮 Delete ", GetCallbackData(CallbackType.RecurringTask, ActionType.Delete, taskId))
                }
            });

        private InlineKeyboardMarkup Emptykeyboard()
            => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>> { });

        private async Task SendKeyboard(Message message, string text, InlineKeyboardMarkup keyboard, bool edit = false)
        {
            if (edit)
            {
                await _bot.EditMessageTextAsync(message.Chat, message.MessageId, text, replyMarkup: keyboard);
            }
            else
            {
                await _bot.SendTextMessageAsync(message.Chat, text, replyMarkup: keyboard);
            }
        }

        private Task SendNotAllowedChatMessage(ChatId id)
            => _bot.SendTextMessageAsync(id, "Sorry, I'm not allowed to talk to you here.");

        private Task SendUnknownUserMessage(ChatId id)
            => _bot.SendTextMessageAsync(id, "Sorry, you're not known to the DB yet");

        private Task SendNotSupportedMessage(ChatId id)
            => _bot.SendTextMessageAsync(id, "Action not supported yet");

        // Process Inline Keyboard callback data
        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            try
            {
                var callbackData = JsonConvert.DeserializeObject<CallbackData>(callbackQueryEventArgs.CallbackQuery.Data);

                var chatId = callbackQueryEventArgs.CallbackQuery.Message.Chat;
                var oldMessage = callbackQueryEventArgs.CallbackQuery.Message;

                switch (callbackData.CallbackType)
                {
                    case CallbackType.RecurringTask:
                        {
                            switch (callbackData.ActionType)
                            {
                                case ActionType.Accept:
                                    {
                                        var data = DeserializeHumanReadable(oldMessage.Text);

                                        await _recurringTaskRepository.Insert(new Common.Db.Models.RecurringTask
                                        {
                                            IntervalDays = int.Parse(data[0]),
                                            Name = data[1],
                                            Description = data[2]
                                        });
                                        break;
                                    }
                                case ActionType.Dismiss:
                                    {
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        break;
                                    }
                                case ActionType.Edit:
                                    {
                                        await SendNotSupportedMessage(oldMessage.Chat);
                                        break;
                                    }
                                case ActionType.Delete:
                                    {
                                        await _recurringTaskRepository.Delete(callbackData.Id.Value);
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        break;
                                    }
                                default:
                                    {
                                        await SendNotSupportedMessage(chatId);
                                        break;
                                    }
                            }
                            break;
                        }
                    case CallbackType.ScheduledTask:
                        {
                            switch (callbackData.ActionType)
                            {
                                case ActionType.Accept:
                                    {
                                        var data = DeserializeHumanReadable(oldMessage.Text);

                                        await _scheduledRepository.Insert(new Common.Db.Models.ScheduledTaskViewModel
                                        {
                                            Date = DateTime.Today.AddDays(int.Parse(data[0])),
                                            Name = data[1],
                                            Description = string.Empty
                                        });
                                        await _bot.EditMessageTextAsync(oldMessage.Chat, oldMessage.MessageId, text: oldMessage.Text, replyMarkup: default);
                                        break;
                                    }
                                case ActionType.Dismiss:
                                    {
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        break;
                                    }
                                case ActionType.Delete:
                                    {
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        await _scheduledRepository.Delete(callbackData.Id.Value);
                                        break;
                                    }
                                case ActionType.Assign:
                                    {
                                        var users = await _userRepository.GetAll();
                                        var telegramId = callbackQueryEventArgs.CallbackQuery.From.Id;
                                        var user = users.FirstOrDefault(x => x.TelegramId == telegramId);

                                        if (user == null)
                                        {
                                            await SendUnknownUserMessage(chatId);
                                            break;
                                        }
                                        var taskId = callbackData.Id.Value;

                                        await _scheduledRepository.SetAssignedUser(taskId, user.Id);
                                        await this.SendKeyboard(oldMessage,
                                                                await GetMessageTextForScheduledMessage(taskId),
                                                                ScheduledMessageAssignedKeyboard(taskId),
                                                                true);
                                        break;
                                    }
                                case ActionType.Unassign:
                                    {
                                        var taskId = callbackData.Id.Value;

                                        await _scheduledRepository.SetAssignedUser(taskId, -1);
                                        await this.SendKeyboard(oldMessage,
                                                                await GetMessageTextForScheduledMessage(taskId),
                                                                ScheduledMessageDefaultKeyboard(taskId),
                                                                true);
                                        break;
                                    }
                                case ActionType.Done:
                                    {
                                        var taskId = callbackData.Id.Value;

                                        await _scheduledRepository.SetDone(taskId);
                                        await this.SendKeyboard(oldMessage,
                                                                await GetMessageTextForScheduledMessage(taskId),
                                                                Emptykeyboard(),
                                                                true);
                                        break;
                                    }
                                default:
                                    {
                                        await SendNotSupportedMessage(chatId);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                await _bot.SendTextMessageAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat, e.ToString());
            }
        }

        private string GetMessageCommand(string messageString)
        {
            return messageString.Split(' ')[0].Split('@')[0];
        }

        private string GetMessageContent(string messageString)
        {
            var splitMessage = messageString.Split(' ');
            return splitMessage.Length <= 1 ? string.Empty : string.Join(" ", splitMessage.Skip(1));
        }

        private string SerializeHumanReadable(IList<string> strings)
        {
            return string.Join(" | ", strings.Select(x => x.Replace("|", string.Empty)));
        }

        private IList<string> DeserializeHumanReadable(string str)
        {
            return str
                .Split(new string[] { "|" }, StringSplitOptions.None)
                .Select(x => x.Trim())
                .ToList();
        }

        private string GetCallbackData(CallbackType callbackType, ActionType actionType, int? id = default)
        {
            var callbackData = new CallbackData
            {
                CallbackType = callbackType,
                ActionType = actionType,
                Id = id
            };
            return JsonConvert.SerializeObject(callbackData);
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}