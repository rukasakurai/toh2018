using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class RegisterRequest
    {
        public string teamId { get; set; }
        public List<Member> members { get; set; }

        public RegisterRequest()
        {
            members = new List<Member>();
        }
    }
}