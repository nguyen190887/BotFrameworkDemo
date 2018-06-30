using BotFrameworkDemo.Extensions;
using BotFrameworkDemo.Models;
using BotFrameworkDemo.Processors;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace BotFrameworkDemo.Dialogs
{
    [Serializable]
    public class BettingDialog : IDialog<object>
    {
        public BotMessages Messages { get; set; }

        private BetProcessor _processor;

        public BettingDialog(BotMessages messages)
        {
            Messages = messages;
            _processor = new BetProcessor { Messages = Messages };
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Messages.GreetingMessages.PickOne());

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;
            await _processor.ProcessBet(context, activity);
        }

        
    }
}
