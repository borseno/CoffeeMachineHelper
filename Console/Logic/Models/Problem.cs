using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        public Question FirstQuestion { get; set; }

        public IEnumerable<Solution> Solutions { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Value { get; set; }
       
        public IEnumerable<Answer> Answers { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public Question Origin { get; set; }
        public Question NextQuestion { get; set; }
        public Solution Solution { get; set; }
    }

    public class Solution
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public Problem Problem { get; set; }
    }
}
