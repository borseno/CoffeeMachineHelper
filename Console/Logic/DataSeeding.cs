using System.Collections.Generic;
using System.Linq;
using Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic
{
    using static DataSupplier;

    internal static class DataSupplier
    {
        public static ICollection<object> GetSolutions()
        {
            var solution1 = new
            {
                ProblemId = -1,
                Id = -1,
                Value = "Ok, then just dont use it for 5 mins."
            };

            var solutions = new object[]
            {
                solution1              
            };

            return solutions;
        }

        public static ICollection<object> GetAnswers()
        {
            var answer1 = new
            {
                Value = "Maybe it's because I'm a loose dude... 😕",
                Id = -1,
                OriginId = -1,
                NextQuestionId = -2
            };

            var answer2 = new
            {
                Value = "Well, sometimes it did... yeah...",
                Id = -2,
                OriginId = -2,
                NextQuestionId = -3
            };

            var answer3 = new
            {
                Value = "Yes..",
                Id = -3,
                OriginId = -3, 
                SolutionId = -1
            };

            var answers = new object[]
            {
                answer1,
                answer2,
                answer3
            };

            return answers;
        }

        public static ICollection<object> GetProblems()
        {
            var problem = new
            {
                Description = "Machine is not working anymore, although it used to work",
                Name = "Not working machine",
                ShortDescription = "Machine is not working anymore!",
                Id = -1,
                FirstQuestionId = -1
            };

            var problems = new[]
            {
                problem
            };

            return problems;
        }

        public static ICollection<object> GetQuestions()
        {
            var question1 = new Question
            {

                Id = -1,
                Value = "Why do you think it is not working?"
            };

            var question2 = new Question
            {
                Id = -2,
                Value = "Well, that doesnt completely describe why it broke. Has it ever fallen?"
            };

            var question3 = new Question
            {
                Id = -3,
                Value = "Ok... Jerk your machine a bit. Do you hear a sound of something's falling up and down?"
            };

            var array = new[]
            {
                question1,
                question2,
                question3
            };

            return array;
        }

        public static ICollection<object> GetKeys()
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

            var keys = strs.Select(i => new Key { Value = i }).ToArray();

            return keys;
        }
    }

    internal static class DataSeeder
    {
        public static void Seed(ModelBuilder blder)
        {
            SeedKeys(blder);
            SeedQuestions(blder);
            SeedProblems(blder);
            SeedAnswers(blder);
            SeedSolutions(blder);
        }

        public static void SeedProblems(ModelBuilder blder)
        {
            var problems = GetProblems();

            blder.Entity<Problem>()
                .HasData(problems);
        }
        public static void SeedSolutions(ModelBuilder blder)
        {
            var solutions = GetSolutions();

            blder.Entity<Solution>()
                .HasData(solutions);
        }

        public static void SeedAnswers(ModelBuilder blder)
        {
            var answers = GetAnswers();

            blder.Entity<Answer>()
                .HasData(answers);
        }

        public static void SeedQuestions(ModelBuilder blder)
        {
            var questions = GetQuestions();

            blder.Entity<Question>()
                .HasData(questions);
        }
        public static void SeedKeys(ModelBuilder blder)
        {
            var keys = GetKeys();

            blder.Entity<Key>()
                .HasData(keys);
        }
    }
}
