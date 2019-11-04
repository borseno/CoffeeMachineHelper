namespace Logic.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public Question Origin { get; set; }
        public Question NextQuestion { get; set; }
        public Solution Solution { get; set; }
    }
}
