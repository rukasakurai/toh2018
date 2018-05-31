using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class Question
    {
        public int id { get; set; }
        public string text { get; set; }
        public List<QuestionOption> questionOptions { get; set; }

        public List<string> GetQuestionOptionTextList()
        {
            List<string> optionTextList = new List<string>();
            foreach (QuestionOption option in questionOptions)
            {
                optionTextList.Add(option.text);
            }
            return optionTextList;
        }
    }

}