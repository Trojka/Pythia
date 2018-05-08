using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace SceneBuilderBot.Dialogs
{
    [Serializable]
    public class StateSavingMessagingDialog : IDialog<object>
    {
        private bool userWelcomed;
        private string userId;
        private string currentSceneId;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(ConversationMessages.WelcomeMessage);
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;


            if (!context.UserData.ContainsKey(ConversationStorageKeys.UserNameId))
            {
                PromptDialog.Text(context, this.HandleUserIdPrompt, "Before we get started, please tell me your name?");
                return;
            }
            else
            {
                userId = context.UserData.GetValue<string>(ConversationStorageKeys.UserNameId);
            }

            if(!userWelcomed)
            {
                userWelcomed = true;
                await context.PostAsync($"Welcome back {userId}! {ConversationMessages.HelpMessage}");

                context.Wait(this.MessageReceivedAsync);
                return;
            }

            if (activity.Text.Equals(ConversationKeySentences.StartNewScene, StringComparison.InvariantCultureIgnoreCase))
            {
                PromptDialog.Text(context, this.HandleSceneNamePrompt, "Give a name for your scene.");
                return;
            }


            context.Wait(MessageReceivedAsync);
        }

        private async Task HandleUserIdPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                userId = await result;
                this.userWelcomed = true;

                await context.PostAsync($"Welcome {userId}!");

                context.UserData.SetValue(ConversationStorageKeys.UserNameId, userId);
            }
            catch (TooManyAttemptsException)
            {
            }

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task HandleSceneNamePrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                currentSceneId = await result;

                await context.PostAsync($"You can now start building scene {currentSceneId}!");

                context.UserData.SetValue(ConversationStorageKeys.CurrentSceneId, currentSceneId);
            }
            catch (TooManyAttemptsException)
            {
            }

            context.Wait(this.MessageReceivedAsync);
        }
    }
}