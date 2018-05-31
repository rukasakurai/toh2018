using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class AnswerResponse
    {
        public bool correct { get; set; }
        public string achievementBadge { get; set; }
        public string achievementBadgeIcon { get; set; }
    }
}