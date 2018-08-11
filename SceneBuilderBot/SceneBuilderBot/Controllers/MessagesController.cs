using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SceneBuilderBot.Dialogs;
using SceneBuilderBot.Logic.Conversations;
using SceneBuilderBot.Services.Repositories;

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
                Debug.WriteLine($"MessagesController.Post > ActivityTypes.Message > ServiceUrl: {activity.ServiceUrl}, ChannelId: {activity.ChannelId}, From: {activity.From.Id}, Conversation: {activity.Conversation.Id}, Text: {activity.Text}");

                StateClient stateClient = activity.GetStateClient();
                BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
                var isKnownUser = userData.GetProperty<bool>(UserStorageKeys.IsKnownUser);
                if(isKnownUser) {
                    Debug.WriteLine("The user is known");
                }
                else {
                    Debug.WriteLine("The user is not known");

                    BotData useConversationData = stateClient.BotState.GetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id);
                    useConversationData.SetProperty<bool>(ConversationStorageKeys.TakeTour, TakeTourConversationParser.TakeTour(activity.Text));
                    stateClient.BotState.SetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id, useConversationData);

                    await Conversation.SendAsync(activity, () => new Dialogs.UnknownUserRootDialog());
                }
            }
            else
            {
                Debug.WriteLine("Process all other");
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

                //https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0

                StateClient stateClient = message.GetStateClient();
                BotData userData = stateClient.BotState.GetUserData(message.ChannelId, message.From.Id);

                IConversationUpdateActivity update = message;
                if (update.MembersAdded != null && update.MembersAdded.Any()) {
                    Debug.WriteLine($"MessagesController.HandleSystemMessage > ConversationUpdate > ServiceUrl: {message.ServiceUrl}, ChannelId: {message.ChannelId}, From: {message.From.Id}");
                    var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                    foreach(var newMember in update.MembersAdded) {
                        if (newMember.Id != message.Recipient.Id) {
                            var reply = message.CreateReply();
                            var userRepository = InMemoryUserRepository.Get();
                            if(!userRepository.ChannelUserExists(newMember.Id)) {
                                reply.Text = BotAnswers.WelcomeMessageForNewUser(newMember.Name);

                                userData.SetProperty<bool>("IsKnownUser", false);
                                Debug.WriteLine($"MessagesController.HandleSystemMessage > ConversationUpdate > IsKnownUser = {userData.GetProperty<bool>(UserStorageKeys.IsKnownUser)}");
                            }
                            else {
                                reply.Text = BotAnswers.WelcomeMessageForKnownUser(newMember.Name);

                                userData.SetProperty<bool>("IsKnownUser", true);
                                Debug.WriteLine($"MessagesController.HandleSystemMessage > ConversationUpdate > IsKnownUser = {userData.GetProperty<bool>(UserStorageKeys.IsKnownUser)}");
                            }
                            client.Conversations.ReplyToActivityAsync(reply);

                            //https://stackoverflow.com/questions/43371605/botframework-privateconversationdata-or-userdata-becomes-empty
                            stateClient.BotState.SetUserData(message.ChannelId, message.From.Id, userData);
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