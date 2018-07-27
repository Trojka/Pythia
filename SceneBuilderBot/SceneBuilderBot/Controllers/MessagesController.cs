using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace SceneBuilderBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            Debug.WriteLine("MessagesController.HandleSystemMessage");
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                Debug.WriteLine("MessagesController.HandleSystemMessage > DeleteUserData");
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                Debug.WriteLine("MessagesController.HandleSystemMessage > ConversationUpdate");
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                IConversationUpdateActivity update = message;
                if(update.MembersAdded != null && update.MembersAdded.Any()) {
                    var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                    foreach(var newMember in update.MembersAdded) {
                        if (newMember.Id != message.Recipient.Id) {
                            var reply = message.CreateReply();
                            reply.Text = $"Welcome {newMember.Name}!";
                            client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                Debug.WriteLine("MessagesController.HandleSystemMessage > ContactRelationUpdate");
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                Debug.WriteLine("MessagesController.HandleSystemMessage > Typing");
                // Handle knowing that the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
                Debug.WriteLine("MessagesController.HandleSystemMessage > Ping");
            }

            return null;
        }
    }
}