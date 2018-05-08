using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Diagnostics;

namespace SceneBuilderBot.Dialogs
{
    [Serializable]
    public class BasicMessagingDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            Debug.WriteLine("BasicMessagingDialog.StartAsync > CompletedTask");

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Debug.WriteLine("BasicMessagingDialog.MessageReceivedAsync > await result");

            var activity = await result as Activity;

            Debug.WriteLine("BasicMessagingDialog.MessageReceivedAsync > await result returned");

            // Following is a helper method which creates a message and fills the Text property
            //await context.PostAsync(GetAnswer(activity.Text));
            var message = context.MakeMessage();
            message.Text = GetAnswer(activity.Text);

            Debug.WriteLine("BasicMessagingDialog.MessageReceivedAsync > PostAsync(message)");

            await context.PostAsync(message);

            Debug.WriteLine("BasicMessagingDialog.MessageReceivedAsync > Wait(MessageReceivedAsync)");

            context.Wait(MessageReceivedAsync);

            Debug.WriteLine("BasicMessagingDialog.MessageReceivedAsync > Wait(MessageReceivedAsync) returned");
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