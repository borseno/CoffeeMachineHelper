using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace Logic.Models
{
    public class UserInfo
    {      
        public int UserId { get; set; }
        public User User { get; set; } 

        public bool IsVerified { get; set; }

        public DateTime? UtcVerifyDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
