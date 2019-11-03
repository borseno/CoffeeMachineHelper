using Logic.Models;
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
    public static class Extensions
    {
        public static void FormButtonsAndText(this IEnumerable<Problem> problems, out InlineKeyboardMarkup markup, out string text)
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

        public static Task NotifyAboutKeySuccess(this TelegramBotClient client, long chatId, int msgId, string key = null)
        {
            return client.SendTextMessageAsync(chatId, $"the key you provided {(key == null ? "" : $"{{{key}}}")} has been activated for your account", replyToMessageId: msgId);
        }

        public static Task<Message> HandleNonSuccessKey(this TelegramBotClient client, long chatId, int msgId, string key = null)
        {
            return client.SendTextMessageAsync(chatId, $"the key you provided {(key == null ? "" : $"{{{key}}}")} does not exist", replyToMessageId: msgId);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int columns)
        {
            var result = new List<List<T>>();

            int count = 0;
            var currentList = new List<T>(columns);
            foreach (var i in enumerable)
            {
                if (count == columns)
                {
                    result.Add(currentList);
                    currentList = new List<T>(columns);
                    count = 0;
                }
                currentList.Add(i);
                count++;
            }

            result.Add(currentList);

            return result;
        } 
    }
}
