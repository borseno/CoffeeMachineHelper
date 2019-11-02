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

            var strs = new[]
            {
                "306ec960-34a5-4d6d-91e5-edfe64d0d74a",
                "32629b5d-b986-49e5-b97a-5389ff0e92e1",
                "a597c89e-29df-4519-a194-6ea4dc932992",
                "3d01eb06-3221-46da-9268-7015b7beadfd",
                "37c25c4b-3528-4970-8a35-094fbd8408cc",
                "c0370794-6aba-482e-b544-3e28f2eb5856",
                "0c48092e-cae4-4e81-8f4f-6acb426f8a66",
                "872f8bee-3268-4661-8208-932953a12572",
                "bc168fdb-401e-4f27-9b9c-329e73e985f0",
                "b3069483-2f8b-47a8-9722-38ea89ce057f"
            };

            ctx.Keys.AddRange(strs.Select(i => new Key { Value = i}));

            var handler = new UpdateHandler(bot, ctx);

            handler.Start();

            Console.ReadLine();
        }
    }
}
