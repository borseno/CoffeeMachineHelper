using System.Collections.Generic;

namespace Logic.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Value { get; set; }
       
        public ICollection<Answer> Answers { get; set; }
    }
}
