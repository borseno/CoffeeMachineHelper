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
            DbContextOptions<Logic.AppContext> options = GetOptions(true);
            
            ctx = new Logic.AppContext(options);

            ctx.Database.EnsureCreated();

            bot.OnUpdate += Bot_OnUpdate;
            bot.StartReceiving();

            Console.WriteLine("started receiving..");

            Console.ReadLine();
        }

        private static DbContextOptions<Logic.AppContext> GetOptions(bool isDev)
        {
            if (isDev)
            {
                return new DbContextOptionsBuilder<Logic.AppContext>()
                    .UseInMemoryDatabase("myinMemory").Options;
            }
            else
            {
                return new DbContextOptionsBuilder<Logic.AppContext>()
                    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CoffeeMachineHelperLocal;Trusted_Connection=True;MultipleActiveResultSets=true")
                    .Options;
            }
        }

        private static void Bot_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            var query = ctx.Problems.Include(i => i.FirstQuestion)
                            .ThenInclude(i => i.Answers)
                                .ThenInclude(i => i.Origin)
                        .Include(i => i.FirstQuestion)
                            .ThenInclude(i => i.Answers)
                                .ThenInclude(i => i.NextQuestion)
                                    .ThenInclude(i => i.Answers)
                                        .ThenInclude(i => i.Solution)
                        .Include(i => i.FirstQuestion)
                            .ThenInclude(i => i.Answers)
                                .ThenInclude(i => i.Solution);


            var handler = new UpdateHandler(bot, ctx, e.Update);

            try
            {
                handler.HandleUpdate().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
