using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleEchoBot.ApiClients
{
    public static class CallQuestionApi
    {

        private static readonly HttpClient client = new HttpClient();
        private const string Url = "https://msopenhack.azurewebsites.net/api/trivia/";
        public static async Task<Question> GetQuestionAsync(string userId)
        {
            // Call Question API
            userId = "877de47c-5c27-4232-9d50-b3133fbd3905";
            string requestString = "{ \"id\": \"" + userId + "\" }";
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Url + "question", content);

            var responseString = await response.Content.ReadAsStringAsync();

            Question question = JsonConvert.DeserializeObject<Question>(responseString);
            return question;
        }

        public static async Task<Result> PostRegisterAsync(string textRegInfo)
        {
            if (string.IsNullOrWhiteSpace(textRegInfo)) throw new ArgumentException("Null argument");


            var content = new StringContent(textRegInfo, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Url + "register", content);

            var responseString = await response.Content.ReadAsStringAsync();

            Result re = JsonConvert.DeserializeObject<Result>(responseString);
            return re;
        }

    }
}
