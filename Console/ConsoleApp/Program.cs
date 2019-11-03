using Logic;
using Microsoft.EntityFrameworkCore;
using System;
using Telegram.Bot;
using Logic.Models;
using System.Linq;

namespace ConsoleApp
{
    public class Program
    {
        static TelegramBotClient bot;
        static Logic.AppContext ctx;

        public static void Main(string[] args)
        {
            const string token = "959390041:AAFQ0t_HNveMuRaRUrtKMeVKXqAc0v2x8Us";

            bot = new TelegramBotClient(token);

            var ctxOptionsBuilder = 
                new DbContextOptionsBuilder<Logic.AppContext>()
                .UseInMemoryDatabase("myName");

            ctx = new Logic.AppContext(ctxOptionsBuilder.Options);

            ctx.Database.EnsureCreated();

            bot.OnUpdate += Bot_OnUpdate;
            bot.StartReceiving();

            Console.ReadLine();
        }

        private static void Bot_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            var handler = new UpdateHandler(bot, ctx, e.Update);

            handler.HandleUpdate().GetAwaiter().GetResult();
        }
    }
}
