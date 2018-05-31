using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using SimpleEchoBot.Models;
using System;
using System.Web.Script.Serialization;
using SimpleEchoBot.ApiClients;
using Microsoft.Bot.Connector.Teams;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new EchoDialog());
            }
            else
            {
                await HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {

                foreach(var mem in message.MembersAdded)
                {
                       // if the member that was added is the bot, then register team
                    if (mem.Id==message.Recipient.Id)
                    {
                        var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                        var members = (await connector.Conversations.GetConversationMembersAsync(message.Conversation.Id)).AsTeamsChannelAccounts();

        
                        RegisterRequest registerRequest = new RegisterRequest();
                        registerRequest.teamId = message.Conversation.Id;
                        Member memberObj;

                        foreach (var member in members)
                        {
                            memberObj = new Member();
                            memberObj.id = member.Id;
                            memberObj.name = member.Name;
                            registerRequest.members.Add(memberObj);
                        }

                        //JSON.net is more popular
                        string requestString = new JavaScriptSerializer().Serialize(registerRequest);

                        Result result = await CallQuestionApi.PostRegisterAsync(requestString);

                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}