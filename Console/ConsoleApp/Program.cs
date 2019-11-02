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
        public static void Main(string[] args)
        {
            const string token = "959390041:AAFQ0t_HNveMuRaRUrtKMeVKXqAc0v2x8Us";

            var bot = new TelegramBotClient(token);

            var ctxOptionsBuilder = 
                new DbContextOptionsBuilder<Logic.AppContext>()
                .UseInMemoryDatabase("myName");

            using var ctx = new Logic.AppContext(ctxOptionsBuilder.Options);

            ctx.Database.EnsureCreated();

            var handler = new UpdateHandler(bot, ctx);

            handler.Start();

            Console.ReadLine();
        }
    }
}
