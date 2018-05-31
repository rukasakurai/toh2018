using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    [Serializable]
    public class QuestionOption
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}