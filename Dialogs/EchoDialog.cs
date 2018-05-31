using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;

using SimpleEchoBot.Models;
using Newtonsoft.Json;
using Microsoft.Bot.Connector.Teams;
using System.Text;
using SimpleEchoBot.ApiClients;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        protected int currentQuestionId = 0;
        protected List<QuestionOption> currentQuestionOptions;
        protected int currentAnswerId = 0;
        
        private static readonly HttpClient client = new HttpClient();

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            // Call Question API
            string userId = message.ChannelData;//"877de47c-5c27-4232-9d50-b3133fbd3905";
            string requestString = "{ \"id\": \"" + userId + "\" }";
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://msopenhack.azurewebsites.net/api/trivia/question", content);
            var responseString = await response.Content.ReadAsStringAsync();
            Question question = JsonConvert.DeserializeObject<Question>(responseString);
            currentQuestionId = question.id;
            currentQuestionOptions = new List<QuestionOption>(question.questionOptions);

            // Prompting the question and answer choices
            PromptDialog.Choice(
              context: context,
              resume: ChoiceReceivedAsync,
              options: question.GetQuestionOptionTextList(),
              prompt: question.text,
              retry: "Selection not avilable . Please try again. "+question.text,
              promptStyle: PromptStyle.Auto
              );
        }

        private async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            string answer = await result;
            await context.PostAsync("ChoiceReceivedAsync");
            await context.PostAsync(answer);
            // Call Answer API
            // Fetch the members in the current conversation
            var connector = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            var members = await connector.Conversations.GetConversationMembersAsync(context.Activity.Conversation.Id);
            string userId = "877de47c-5c27-4232-9d50-b3133fbd3905";

            // Concatenate information about all members into a string
            /*
            string userId = "";
            foreach (var member in members.AsTeamsChannelAccounts())
            {
                if (member.Id == context.Activity.From.Id) {
                    userId = member.ObjectId;
                    break;
                }
            } */

            AnswerRequest answerRequest = new AnswerRequest();
            answerRequest.userId = userId;
            answerRequest.questionId = currentQuestionId;
            answerRequest.answerId = getQuestionId(answer);

            string requestString = new JavaScriptSerializer().Serialize(answerRequest);
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://msopenhack.azurewebsites.net/api/trivia/answer", content);

            var responseString = await response.Content.ReadAsStringAsync();

            AnswerResponse answerResponse = JsonConvert.DeserializeObject<AnswerResponse>(responseString);

            if (answerResponse.correct)
            {
                await context.PostAsync("Correct!");
            }
            else
            {
                await context.PostAsync("Wrong!");
            }

            requestString = "{ \"id\": \"" + userId + "\" }";
            content = new StringContent(requestString, Encoding.UTF8, "application/json");
            response = await client.PostAsync("https://msopenhack.azurewebsites.net/api/trivia/question", content);
            responseString = await response.Content.ReadAsStringAsync();
            Question question = JsonConvert.DeserializeObject<Question>(responseString);
            currentQuestionId = question.id;
            currentQuestionOptions = new List<QuestionOption>(question.questionOptions);

            // Prompting the question and answer choices
            PromptDialog.Choice(
              context: context,
              resume: ChoiceReceivedAsync,
              options: question.GetQuestionOptionTextList(),
              prompt: question.text,
              retry: "Selection not avilable . Please try again. " + question.text,
              promptStyle: PromptStyle.Auto
              );
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        int getQuestionId (string answer)
        {
            foreach (QuestionOption questionOption in currentQuestionOptions)
            {
                if (questionOption.text.Equals(answer)) {
                    return int.Parse(questionOption.id);
                }
            }

            return 0;
        }
    }
}