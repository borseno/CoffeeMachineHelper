using Logic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;

namespace Logic
{
    public class AppContext : DbContext
    {
        public AppContext() : base()
        {

        }

        public AppContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder blder)
        {
            blder.Entity<Key>()
                .HasKey(i => i.Value);

            blder.Entity<UserInfo>()
                .HasKey(i => i.UserId);

            blder.Entity<UserState>()
                .HasKey(i => i.UserId);

            blder.Entity<UserInfo>()
                .HasOne(i => i.User)
                .WithOne();

            blder.Entity<Question>()
                .HasMany(i => i.Answers)
                .WithOne(i => i.Origin);

            blder.Entity<UserInfo>()
                .HasOne(i => i.State)
                .WithOne(i => i.UserInfo)
                .HasForeignKey<UserState>(i => i.UserId);

            SeedData(blder);

            base.OnModelCreating(blder);
        }

        private void SeedData(ModelBuilder blder)
        {
            SeedKeys(blder);
            SeedProblems(blder);


        }

        private static void SeedProblems(ModelBuilder blder)
        {
            var question = new Question
            {

                Id = -1,
                Value = "How do you wanna fix this?"
            };

            var question2 = new Question
            {
                Id = -2,
                Value = "Bullshit, your solution doesnt seem to be good. Still wanna fix it?"
            };

            blder.Entity<Question>()
                .HasData(new[] { question, question2 });

            var answers = Enumerable.Range(1, 1).Select(i => new
            {
                Value = "Well, in some way.." + -i,
                Id = -i,
                OriginId = question.Id,
                NextQuestionId = question2.Id
            });

            blder.Entity<Answer>()
                .HasData(answers);

            var problems = Enumerable.Range(1, 20).Select(i => new
            {
                Description = "нет кофенет конет кофенет кофефенет кофенет кофенет кофенет кофенет кофенет кофе",
                Name = "мм проблема",
                ShortDescription = "holy crap",
                Id = -i,
                FirstQuestionId = question.Id
            });

            blder.Entity<Problem>()
                .HasData(problems);

            var solution = new
            {
                ProblemId = problems.First().Id,
                Id = -1,
                Value = "Ok, just sit down and wait 5 mins.."
            };

            blder.Entity<Solution>()
                .HasData(solution);

            var lastAnswer = new
            {
                Value = "Yes please do.",
                Id = answers.Last().Id - 1,
                OriginId = question2.Id,
                SolutionId = solution.Id
            };

            blder.Entity<Answer>()
                .HasData(lastAnswer);
        }

        private static void SeedKeys(ModelBuilder blder)
        {
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

            var keys = strs.Select(i => new Key { Value = i });

            blder.Entity<Key>()
                .HasData(keys);
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Solution> Solutions { get; set; }
    }
}
