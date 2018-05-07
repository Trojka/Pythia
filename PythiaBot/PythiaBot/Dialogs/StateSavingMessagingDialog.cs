using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PythiaBot.Dialogs
{
    [Serializable]
    public class StateSavingMessagingDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome stranger. I'm the Pythia, your access to the future.");
            context.Wait(MessageReceivedAsync);

            //return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Following is a helper method which creates a message and fills the Text property
            //await context.PostAsync(GetAnswer(activity.Text));
            var message = context.MakeMessage();
            message.Text = GetAnswer(activity.Text);
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        // With basic question and answer type dialog we need to do some kind of parsing of the message
        //  and then formulate an answer back to the user.
        // The parsing of the message and the formulation of the answer you will need to do yourself.
        private string GetAnswer(string messageText)
        {
            int length = (messageText ?? string.Empty).Length;

            return $"You sent <{messageText}> which was {length} characters";
        }
    }
}