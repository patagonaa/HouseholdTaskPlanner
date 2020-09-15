using HouseholdTaskPlanner.TelegramBot.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

        public async Task Startup()
        {
            var me = await _bot.GetMeAsync();
            Console.Title = me.Username;

            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _bot.OnReceiveError += BotOnReceiveError;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
            _bot.StopReceiving();
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            _logger.LogInformation("{ChatId} {UserId} {messageText}", messageEventArgs.Message.Chat.Id, messageEventArgs.Message.From.Id, messageEventArgs.Message.Text);

            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (message.Chat.Id != _configuration.AllowedChat)
            {
                await SendNotAllowedChatMessage(message.Chat.Id);
                return;
            }

            switch (message.Text.Split(' ').First())
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
            var scheduledTasks = (await _scheduledRepository.GetList())
                    .Where(task => today <= task.Date).ToList();

            if (lookahead.HasValue)
            {
                var lookaheadDate = today + lookahead.Value;
                scheduledTasks = scheduledTasks.Where(task => task.Date <= lookaheadDate && today <= task.Date).ToList();
            }

            var users = (await _userRepository.GetAll()).ToList();

            var taskList = new List<Task>();

            if (scheduledTasks.Count == 0)
            {
                taskList.Add(this.SendKeyboard(message, "No scheduled tasks available", default));
            }

            foreach (var task in scheduledTasks)
            {
                var assignedUserId = task.AssignedUser;
                var assignedUser = assignedUserId.HasValue ? users.Find(user => user.Id == assignedUserId.GetValueOrDefault(-1)) : null;

                string messageText = string.Join(Environment.NewLine,
                                    $"{task.Name} | {(assignedUser != null ? $"Assigned to @{assignedUser.TelegramUsername}" : "Not Assigned")}",
                                    string.Empty,
                                    task.Description);
                if (assignedUser == null)
                {
                    taskList.Add(this.SendKeyboard(message, messageText, ScheduledMessageDefaultKeyboard(task.Id.ToString())));
                    continue;
                }

                taskList.Add(this.SendKeyboard(message, messageText, ScheduledMessageAssignedKeyboard(task.Id.ToString())));
            }

            await Task.WhenAll(taskList);
        }

        private async Task ListRecurringTasks(Message message)
        {
            var recurringTasks = (await _recurringTaskRepository.GetAll());

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
                taskList.Add(this.SendKeyboard(message, messageText, RecurringMessageDefaultKeyboard(task.Id.ToString())));
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
            string[] contents = message.Text.Split('\n');

            string daysStr = contents.Length == 2 ? contents[1].Split(' ').FirstOrDefault() : string.Empty;
            bool canParseDays = uint.TryParse(daysStr, out uint _);

            if (contents.Length < 3 || !canParseDays)
            {
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Use this command to create a new recurring task. Use following structure to do so. Currently only days are supported for scheduling tasks"
                );
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "/createRecurring Vacuum Weekly\n7 (days)\nTo ensure a clean space for living, it is adviseable to vacuum at least once a week commonly used rooms."
                );
                return;
            }

            await SendKeyboard(message,
                               $"{contents[0]} | Every {contents[1]} days \n\n {string.Join(Environment.NewLine, contents.Skip(2))}",
                               RecurringMessageAcceptKeyboard(string.Empty), false
                               );
        }

        private async Task CreateNewScheduledTask(Message message)
        {
            string[] contents = message.Text.Split('\n');

            string daysStr = contents.Length == 2 ? contents[1].Split(' ').FirstOrDefault() : string.Empty;
            bool canParseDays = uint.TryParse(daysStr, out uint _);

            if (contents.Length != 2 || !canParseDays)
            {
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Use this command to schedule a one-off task. Use following structure to do so. Currently only days are supported for scheduling"
                );
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "/createSingle Take special garbage out\n4 (days)"
                );
                return;
            }

            await SendKeyboard(message,
                               $"{contents[0].Replace("/createSingle", "")} | In {contents[1]} days due",
                               ScheduledMessageAcceptKeyboard(string.Empty), false
                               );
        }

        private InlineKeyboardMarkup ScheduledMessageDefaultKeyboard(string taskId)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("✔ I can do that!", InlineDataFormatter.FormatScheduled(ScheduledAction.Assign,taskId)),
                    InlineKeyboardButton.WithCallbackData("🚮 Delete ", InlineDataFormatter.FormatScheduled(ScheduledAction.Delete,taskId))
                }
            });

        private InlineKeyboardMarkup ScheduledMessageAssignedKeyboard(string taskId)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("❌ I can't do that!", InlineDataFormatter.FormatScheduled(ScheduledAction.Unassign,taskId)),
                    InlineKeyboardButton.WithCallbackData("✔ I'm done!", InlineDataFormatter.FormatScheduled(ScheduledAction.Done,taskId)),
                }
            });

        private InlineKeyboardMarkup ScheduledMessageAcceptKeyboard(string data)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("❌ Not what I wanted", InlineDataFormatter.FormatScheduled(ScheduledAction.Dismiss,data)),
                    InlineKeyboardButton.WithCallbackData("✔ Schedule this task", InlineDataFormatter.FormatScheduled(ScheduledAction.Accept,data)),
                }
            });

        private InlineKeyboardMarkup RecurringMessageAcceptKeyboard(string data)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("❌ Not what I wanted", InlineDataFormatter.FormatRecurring(RecurringAction.Dismiss,data)),
                    InlineKeyboardButton.WithCallbackData("✔ Schedule this task", InlineDataFormatter.FormatRecurring(RecurringAction.Accept,data)),
                }
            });

        private InlineKeyboardMarkup RecurringMessageDefaultKeyboard(string taskId)
            => new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("✏ Edit ", InlineDataFormatter.FormatRecurring(RecurringAction.Edit,taskId)),
                    InlineKeyboardButton.WithCallbackData("🚮 Delete ", InlineDataFormatter.FormatRecurring(RecurringAction.Delete,taskId))
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
                string inlineData = callbackQueryEventArgs.CallbackQuery.Data;

                var chatId = callbackQueryEventArgs.CallbackQuery.Message.Chat;
                var oldMessage = callbackQueryEventArgs.CallbackQuery.Message;

                switch (InlineDataFormatter.GetPrefix(inlineData))
                {
                    case InlineDataFormatter.Prefixes.Recurring:
                        {
                            // delete or edit
                            var (action, taskId) = InlineDataFormatter.ParseRecurring(inlineData);
                            switch (action)
                            {
                                case RecurringAction.Accept:
                                    {
                                        string[] content = oldMessage.Text.Split('-');
                                        string title = content[0], days = content[1], description = content[2];
                                        await _recurringTaskRepository.Insert(new Common.Db.Models.RecurringTask
                                        {
                                            Name = title,
                                            Description = description,
                                            IntervalDays = Convert.ToInt32(days)
                                        });
                                        break;
                                    }
                                case RecurringAction.Dismiss:
                                    {
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        break;
                                    }
                                case RecurringAction.Edit:
                                    {
                                        await SendNotSupportedMessage(oldMessage.Chat);
                                        break;
                                    }
                                case RecurringAction.Delete:
                                    {
                                        await _recurringTaskRepository.Delete(taskId.Value);
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        break;
                                    }
                                default:
                                    { break; }
                            }
                            break;
                        }
                    case InlineDataFormatter.Prefixes.Scheduled:
                        {
                            var (action, taskId) = InlineDataFormatter.ParseScheduled(inlineData);

                            switch (action)
                            {
                                case ScheduledAction.Accept:
                                    {
                                        string[] content = oldMessage.Text.Split('\n');
                                        string[] titleContents = content[0].Split('|');
                                        string title = titleContents[0],
                                               days = Convert.ToInt32(titleContents[1].Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries)[1]).ToString();
                                        await _scheduledRepository.Insert(new Common.Db.Models.ScheduledTaskViewModel
                                        {
                                            Name = title,
                                            Date = DateTime.Today.AddDays(Convert.ToInt32(days)),
                                            Description = string.Empty
                                        });
                                        await _bot.EditMessageTextAsync(oldMessage.Chat, oldMessage.MessageId, text: oldMessage.Text, replyMarkup: default);
                                        break;
                                    }
                                case ScheduledAction.Dismiss:
                                    {
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        break;
                                    }
                                case ScheduledAction.Delete:
                                    {
                                        await _bot.DeleteMessageAsync(oldMessage.Chat, oldMessage.MessageId);
                                        await _scheduledRepository.Delete(taskId.Value);
                                        break;
                                    }
                                case ScheduledAction.Assign:
                                    {
                                        var users = await _userRepository.GetAll();
                                        var telegramId = callbackQueryEventArgs.CallbackQuery.From.Id;
                                        var user = users.FirstOrDefault(x => x.TelegramId == telegramId);

                                        if (user == null)
                                        {
                                            await SendUnknownUserMessage(chatId);
                                            break;
                                        }

                                        await _scheduledRepository.SetAssignedUser(taskId.Value, user.Id);
                                        await this.SendKeyboard(oldMessage,
                                                                await GetMessageTextForScheduledMessage(taskId.Value),
                                                                ScheduledMessageAssignedKeyboard(taskId.ToString()),
                                                                true);
                                        break;
                                    }
                                case ScheduledAction.Unassign:
                                    {
                                        await _scheduledRepository.SetAssignedUser(taskId.Value, -1);
                                        await this.SendKeyboard(oldMessage,
                                                                await GetMessageTextForScheduledMessage(taskId.Value),
                                                                ScheduledMessageDefaultKeyboard(taskId.ToString()),
                                                                true);
                                        break;
                                    }
                                case ScheduledAction.Done:
                                    {
                                        await _scheduledRepository.SetDone(taskId.Value);
                                        await this.SendKeyboard(oldMessage,
                                                                await GetMessageTextForScheduledMessage(taskId.Value),
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

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}