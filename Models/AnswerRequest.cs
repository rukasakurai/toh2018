using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class AnswerRequest
    {
        public string userId { get; set; }
        public int questionId { get; set; }
        public int answerId { get; set; }
    }
}