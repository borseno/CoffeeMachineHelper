using Logic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Logic
{
    public class UpdateHandler
    {
        private readonly TelegramBotClient client;
        private readonly AppContext ctx;

        public UpdateHandler(TelegramBotClient client, AppContext ctx)
        {
            this.client = client;
            this.ctx = ctx;
        }

        public void Start()
        {
            client.StartReceiving();
            client.OnUpdate += Client_OnUpdate;
        }

        private void Client_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
                HandleUpdate(e.Update).GetAwaiter().GetResult();
        }

        public async Task HandleUpdate(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    await HandleMessage(update.Message);
                    break;
                case UpdateType.InlineQuery:
                    break;
                case UpdateType.ChosenInlineResult:
                    break;
                case UpdateType.CallbackQuery:
                    await HandleCallbackQuery(update.CallbackQuery);
                    break;
                case UpdateType.EditedMessage:
                    break;
                case UpdateType.ChannelPost:
                    break;
                case UpdateType.EditedChannelPost:
                    break;
                case UpdateType.ShippingQuery:
                    break;
                case UpdateType.PreCheckoutQuery:
                    break;
                case UpdateType.Poll:
                    break;
                default:
                    break;
            }

            await SaveChangesAsync();
        }

        private async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            var user = callbackQuery.From;
            var userInfo = await ctx.UserInfos.FirstOrDefaultAsync(i => i.UserId == user.Id);

            var stage = userInfo.State.UserStage;

            switch (stage)
            {
                case UserStage.None:
                    break;
                case UserStage.PickingProblem:
                    await HandlePickingProblemUserStage(userInfo, callbackQuery);
                    return;
                case UserStage.AnsweringQuestion:
                    break;
                default:
                    break;
            }
        }

        private async Task HandlePickingProblemUserStage(UserInfo userInfo, CallbackQuery callbackQuery)
        {
            var data = callbackQuery.Data;

            var id = Int32.Parse(data);

            var problem = await ctx.Problems.FirstAsync(i => i.Id == id);

            var question = problem.FirstQuestion;

            question ??= new Question
            {
                Value = "Dude whatare we gonna do omg??",

                Answers = new List<Answer>
                {
                    new Answer
                    {
                        Value = "Sup dude!"
                    }
                }
            };

            await AskQuestion(callbackQuery.Message.Chat.Id, userInfo, question);

            userInfo.State.UserStage = UserStage.AnsweringQuestion;
            userInfo.State.Problem = problem;
            userInfo.State.Question = problem.FirstQuestion;

        }

        private async Task AskQuestion(long chatId, UserInfo userInfo, Question firstQuestion)
        {
            var answers = firstQuestion.Answers
                .Take(10)
                .Select(i => new InlineKeyboardButton
                {
                    CallbackData = i.Id.ToString(),
                    Text = i.Value
                })
                .Split(2);

            var lastRow = new InlineKeyboardButton[2]
            {
                new InlineKeyboardButton
                {
                    Text = "Prev",
                    CallbackData = $"Prev#TODO"
                },
                new InlineKeyboardButton
                {
                    Text = "Next",
                    CallbackData = $"Next#TODO"
                }
            };

            var markup = new InlineKeyboardMarkup(answers.Append(lastRow));

            await client.SendTextMessageAsync(chatId, firstQuestion.Value, replyMarkup: markup);
        }

        private Task SaveChangesAsync()
        {
            return ctx.SaveChangesAsync();
        }

        private async Task HandleMessage(Message message)
        {
            var msg = message;
            var botUser = msg?.From;
            var userInfo = await ctx.UserInfos.FirstOrDefaultAsync(i => i.User.Id == botUser.Id);
            var isFirstRequestByUser = userInfo == null;

            if (isFirstRequestByUser)
            {
                userInfo = InitNewUser(botUser);
            }

            if (userInfo.IsVerified)
            {
                await HandleVerifiedUpdate(userInfo, message);
            }
            else
            {
                await TryAuthenticate(msg, userInfo, isFirstRequestByUser);
            }
        }

        private UserInfo InitNewUser(User botUser)
        {
            UserInfo userInfo = new UserInfo
            {
                User = botUser,
            };
            ctx.UserInfos.Add(userInfo);
            return userInfo;
        }

        private async Task TryAuthenticate(Message msg, UserInfo userInfo, bool isFirstRequest)
        {
            var key = msg?.Text;

            if (string.IsNullOrEmpty(key))
            {
                await HandleNonSuccessKey(msg.Chat.Id, msg.MessageId, key);
                return;
            }

            var keyEntity = await ctx.Keys.FirstOrDefaultAsync(i => i.Value == key);

            if (keyEntity == null)
            {
                await HandleNonSuccessKey(msg.Chat.Id, msg.MessageId, key);
                return;
            }

            userInfo.UtcVerifyDate = DateTime.UtcNow;
            userInfo.UtcExpireDate = userInfo.UtcVerifyDate.Value + keyEntity.Term;

            ctx.Keys.Remove(keyEntity);

            if (!isFirstRequest)
            {
                ctx.Entry(userInfo).State = EntityState.Modified;
            }

            await NotifyAboutKeySuccess(msg.Chat.Id, msg.MessageId, key);
        }

        private Task NotifyAboutKeySuccess(long chatId, int msgId, string key = null)
        {
            return client.SendTextMessageAsync(chatId, $"the key you provided {(key == null ? "" : $"{{{key}}}")} has been activated for your account", replyToMessageId: msgId);
        }

        private Task<Message> HandleNonSuccessKey(long chatId, int msgId, string key = null)
        {
            return client.SendTextMessageAsync(chatId, $"the key you provided {(key == null ? "" : $"{{{key}}}")} does not exist", replyToMessageId: msgId);
        }

        private async Task HandleVerifiedUpdate(UserInfo userInfo, Message message)
        {
            var msg = message;

            if (msg.Text == "/help")
            {
                await SendListOfProblems(userInfo, message.Chat.Id);
            }
        }

        private async Task SendListOfProblems(UserInfo userInfo, long chatId)
        {
            var problems = await ctx.Problems.Take(10).ToArrayAsync();

            FormButtonsAndText(problems, out InlineKeyboardMarkup markup, out string text);

            await client.SendTextMessageAsync(chatId, text, replyMarkup: markup);

            userInfo.State.UserStage = UserStage.PickingProblem;
        }

        private void FormButtonsAndText(IEnumerable<Problem> problems, out InlineKeyboardMarkup markup, out string text)
        {
            var firstId = problems.First().Id;
            var lastId = problems.Last().Id;

            var sb = new StringBuilder();

            foreach (var i in problems)
            {
                sb.AppendLine($"Id: {i.Id} Name: {i.Name} Short Description: {i.ShortDescription}");
            }

            text = sb.ToString();

            var buttons = problems.Select(i => new InlineKeyboardButton
            {
                CallbackData = $"{i.Id}",
                Text = $"{i.Id}. {i.Name}"
            }).Split(2);

            var lastRow = new InlineKeyboardButton[2]
            {
                new InlineKeyboardButton
                {
                    Text = "Prev",
                    CallbackData = $"Prev#{firstId}"
                },
                new InlineKeyboardButton
                {
                    Text = "Next",
                    CallbackData = $"Next#{lastId}"
                }
            };

            markup = new InlineKeyboardMarkup(buttons.Append(lastRow));
        }
    }
}
