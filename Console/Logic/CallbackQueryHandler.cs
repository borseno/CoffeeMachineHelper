using Logic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Logic
{
    class CallbackQueryHandler
    {
        private readonly TelegramBotClient client;
        private readonly AppContext ctx;
        private readonly CallbackQuery callbackQuery;

        public CallbackQueryHandler(TelegramBotClient client, AppContext ctx, CallbackQuery query)
        {
            this.client = client;
            this.ctx = ctx;
            this.callbackQuery = query;
        }

        public async Task HandleCallbackQuery()
        {
            var user = callbackQuery.From;
            var userInfo = await ctx.UserInfos.FirstOrDefaultAsync(i => i.UserId == user.Id);

            var stage = userInfo.State.UserStage;

            switch (stage)
            {
                case UserStage.None:
                    break;
                case UserStage.PickingProblem:
                    await HandlePickingProblemUserStage(userInfo);
                    return;
                case UserStage.AnsweringQuestion:
                    break;
                default:
                    break;
            }
        }

        private async Task HandlePickingProblemUserStage(UserInfo userInfo)
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
    }
}
