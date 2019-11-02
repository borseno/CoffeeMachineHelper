using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace Logic.Models
{
    public class UserInfo
    {   
        public UserInfo()
        {
            State ??= new UserState
            {
                UserInfo = this
            };
        }

        public int UserId { get; set; }
        public User User { get; set; }

        public bool IsVerified => UtcExpireDate == null ? false : DateTime.UtcNow > UtcExpireDate.Value;
        public DateTime? UtcVerifyDate { get; set; }
        public DateTime? UtcExpireDate { get; set; }

        public UserState State { get; set; }
    }

    public class UserState
    {
        public int UserId { get; set; }
        public UserInfo UserInfo { get; set; }

        public UserStage UserStage { get; set; }

        public Problem Problem { get; set; }
        public Question Question { get; set; }
    }

    public enum UserStage
    {
        None = 0,
        PickingProblem = 1,
        AnsweringQuestion = 2
    }
}
