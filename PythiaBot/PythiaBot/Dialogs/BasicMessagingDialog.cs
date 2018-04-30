using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PythiaBot.Dialogs
{
    [Serializable]
    public class BasicMessagingDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await context.PostAsync(GetAnswer(activity.Text));
            context.Wait(MessageReceivedAsync);
        }

        private string GetAnswer(string messageText)
        {
            int length = (messageText ?? string.Empty).Length;

            return $"You sent <{messageText}> which was {length} characters";
        }
    }
}