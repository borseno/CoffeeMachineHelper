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
    class MessageHandler
    {
        private readonly TelegramBotClient client;
        private readonly AppContext ctx;
        private readonly Message msg;

        public MessageHandler(TelegramBotClient client, AppContext ctx, Message update)
        {
            this.client = client;
            this.ctx = ctx;
            this.msg = update;
        }

        public async Task HandleMessage()
        {
            var msg = this.msg;
            var botUser = msg?.From;
            var userInfo = await ctx.UserInfos.FirstOrDefaultAsync(i => i.User.Id == botUser.Id);
            var isFirstRequestByUser = userInfo == null;

            if (isFirstRequestByUser)
            {
                userInfo = InitNewUser(botUser);
            }

            if (userInfo.IsVerified)
            {
                await HandleVerifiedUpdate(userInfo);
            }
            else
            {
                await TryAuthenticate(userInfo, isFirstRequestByUser);
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

        private async Task TryAuthenticate(UserInfo userInfo, bool isFirstRequest)
        {
            var key = msg?.Text;

            if (string.IsNullOrEmpty(key))
            {
                await client.HandleNonSuccessKey(msg.Chat.Id, msg.MessageId, key);
                return;
            }

            var keyEntity = await ctx.Keys.FirstOrDefaultAsync(i => i.Value == key);

            if (keyEntity == null)
            {
                await client.HandleNonSuccessKey(msg.Chat.Id, msg.MessageId, key);
                return;
            }


            userInfo.UtcVerifyDate = DateTime.UtcNow;
            userInfo.UtcExpireDate = userInfo.UtcVerifyDate.Value + keyEntity.Term;

            ctx.Keys.Remove(keyEntity);

            if (!isFirstRequest)
            {
                ctx.Entry(userInfo).State = EntityState.Modified;
            }

            await client.NotifyAboutKeySuccess(msg.Chat.Id, msg.MessageId, key);
        }

        private async Task HandleVerifiedUpdate(UserInfo userInfo)
        {
            var msg = this.msg;

            if (msg.Text == "/help")
            {
                await SendListOfProblems(userInfo, this.msg.Chat.Id);
            }
        }

        private async Task SendListOfProblems(UserInfo userInfo, long chatId)
        {
            var problems = await ctx.Problems.Take(10).ToArrayAsync();

            problems.FormButtonsAndText(out InlineKeyboardMarkup markup, out string text);

            await client.SendTextMessageAsync(chatId, text, replyMarkup: markup);

            userInfo.State.UserStage = UserStage.PickingProblem;
        }
    }
}
