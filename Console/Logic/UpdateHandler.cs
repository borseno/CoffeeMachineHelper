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
        private readonly Update update;

        public UpdateHandler(TelegramBotClient client, AppContext ctx, Update update)
        {
            this.client = client;
            this.ctx = ctx;
            this.update = update;
        }

        public async Task HandleUpdate()
        {
            switch (update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    var msghandler = new MessageHandler(client, ctx, update.Message);
                    await msghandler.HandleMessage();
                    break;
                case UpdateType.InlineQuery:
                    break;
                case UpdateType.ChosenInlineResult:
                    break;
                case UpdateType.CallbackQuery:
                    var callBackHandler = new CallbackQueryHandler(client, ctx, update.CallbackQuery);
                    await callBackHandler.HandleCallbackQuery();
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

        private Task SaveChangesAsync()
        {
            return ctx.SaveChangesAsync();
        }
    }
}
