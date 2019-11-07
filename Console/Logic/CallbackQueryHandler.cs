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

            var problem = await ctx.Problems
                .Include(i => i.FirstQuestion)
                    .ThenInclude(i => i.Answers)
                .FirstAsync(i => i.Id == id);

            var question = problem.FirstQuestion;

            InlineKeyboardMarkup markup = GetMarkup(question, 10, 0);

            await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, question.Value, replyMarkup: markup);

            userInfo.State.UserStage = UserStage.AnsweringQuestion;
            userInfo.State.Problem = problem;
            userInfo.State.Question = problem.FirstQuestion;
        }

        private static InlineKeyboardMarkup GetMarkup(Question question, int length, int offset = 0)
        {
            var answers = question.Answers.Skip(offset).Take(length);

            var answersButtons = answers.Select(i => new InlineKeyboardButton
            {
                CallbackData = i.Id.ToString(),
                Text = i.Value
            }).Split(2);

            var lastRow = new InlineKeyboardButton[2]
            {
                new InlineKeyboardButton
                {
                    Text = "Prev",
                    CallbackData = $"Prev#{question.Id}#{length}#{offset}"
                },
                new InlineKeyboardButton
                {
                    Text = "Next",
                    CallbackData = $"Next#{question.Id}#{length}#{offset}"
                }
            };

            var buttonsWithAppended = answersButtons.Append(lastRow);

            var markup = new InlineKeyboardMarkup(buttonsWithAppended);

            return markup;
        }
    }
}
