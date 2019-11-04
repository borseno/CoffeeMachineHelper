namespace Logic.Models
{
    public class UserState
    {
        public int UserId { get; set; }
        public UserInfo UserInfo { get; set; }

        public UserStage UserStage { get; set; }

        public Problem Problem { get; set; }
        public Question Question { get; set; }
    }
}
