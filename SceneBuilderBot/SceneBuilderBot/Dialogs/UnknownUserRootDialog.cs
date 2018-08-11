using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SceneBuilderBot.Services.Repositories;
using System.Diagnostics;

namespace SceneBuilderBot.Dialogs
{
    [Serializable]
    public class UnknownUserRootDialog : IDialog<object>
    {
        private string userId;
        private string currentSceneId;

        public async Task StartAsync(IDialogContext context)
        {
            Debug.WriteLine("UnknownUserRootDialog.StartAsync > Enter");
            await context.PostAsync(ConversationMessages.WelcomeMessage);
            Debug.WriteLine("UnknownUserRootDialog.StartAsync > After context.PostAsync / Before context.Wait");
            context.Wait(MessageReceivedAsync);
            Debug.WriteLine("UnknownUserRootDialog.StartAsync > After context.Wait");
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > Start");
            var activity = await result as Activity;


            if (!context.UserData.ContainsKey(ConversationStorageKeys.UserNameId))
            {
                Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > Prompt 'Before we get started'");
                PromptDialog.Text(context, this.HandleUserIdPrompt, "Before we get started, please tell me your name?");
                Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > return");
                return;
            }
            else
            {
                userId = context.UserData.GetValue<string>(ConversationStorageKeys.UserNameId);
            }

            if (activity.Text.Equals(ConversationKeySentences.StartNewScene, StringComparison.InvariantCultureIgnoreCase))
            {
                Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > Prompt 'Before we get started'");
                PromptDialog.Text(context, this.HandleSceneNamePrompt, "Give a name for your scene.");
                Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > return");
                return;
            }

            Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > Before context.Wait");
            context.Wait(MessageReceivedAsync);
            Debug.WriteLine("UnknownUserRootDialog.MessageReceivedAsync > After context.Wait");
        }

        private async Task HandleUserIdPrompt(IDialogContext context, IAwaitable<string> result)
        {
            Debug.WriteLine("UnknownUserRootDialog.HandleUserIdPrompt > Start");
            try {
                var userName = await result;

                Debug.WriteLine("UnknownUserRootDialog.HandleUserIdPrompt > Before context.PostAsync");
                await context.PostAsync($"Welcome {userId}!");
                Debug.WriteLine("UnknownUserRootDialog.HandleUserIdPrompt > After context.PostAsync");

                var userRepository = InMemoryUserRepository.Get();
                userId = userRepository.RegisterChannelUser(context.Activity.From.Id, userName);

                context.UserData.SetValue(ConversationStorageKeys.UserNameId, userId);
            }
            catch (TooManyAttemptsException)
            {
            }

            Debug.WriteLine("UnknownUserRootDialog.HandleUserIdPrompt > Before context.Wait");
            context.Wait(this.MessageReceivedAsync);
            Debug.WriteLine("UnknownUserRootDialog.HandleUserIdPrompt > After context.Wait");
        }

        private async Task HandleSceneNamePrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                currentSceneId = await result;

                Debug.WriteLine("UnknownUserRootDialog.HandleSceneNamePrompt > Before context.PostAsync");
                await context.PostAsync($"You can now start building scene {currentSceneId}!");
                Debug.WriteLine("UnknownUserRootDialog.HandleSceneNamePrompt > After context.PostAsync");

                context.UserData.SetValue(ConversationStorageKeys.CurrentSceneId, currentSceneId);
            }
            catch (TooManyAttemptsException)
            {
            }

            Debug.WriteLine("UnknownUserRootDialog.HandleSceneNamePrompt > Before context.Wait");
            context.Wait(this.MessageReceivedAsync);
            Debug.WriteLine("UnknownUserRootDialog.HandleSceneNamePrompt > After context.Wait");
        }
    }
}