using Logic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            await DoWorkAsync(update);
            await SaveChangesAsync();
        }

        private Task SaveChangesAsync()
        {
            return ctx.SaveChangesAsync();
        }

        private async Task DoWorkAsync(Update update)
        {
            var msg = update.Message;
            var botUser = msg?.From;
            var userInfo = await ctx.UserInfos.FirstOrDefaultAsync(i => i.User.Id == botUser.Id);

            if (userInfo == null)
            {
                userInfo = InitNewUser(botUser);
            }

            if (userInfo.IsVerified)
            {
                await HandleVerifiedUpdate(update, userInfo);
            }
            else
            {
                await TryAuthenticate(msg, userInfo);
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

        private async Task TryAuthenticate(Message msg, UserInfo userInfo)
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

            ctx.Entry(userInfo).State = EntityState.Modified;

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

        private async Task HandleVerifiedUpdate(Update update, UserInfo userInfo)
        {
            
        }
    }
}
