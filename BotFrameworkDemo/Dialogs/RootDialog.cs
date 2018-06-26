using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BotFrameworkDemo.Models;
using BotFrameworkDemo.Processors;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BotFrameworkDemo.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private Lazy<BotMessages> MessagesLazy = new Lazy<BotMessages>(() => HttpContext.Current.Application["BotMessages"] as BotMessages);

        private TeamCounter _teamCounter = new TeamCounter();

        private GreetingHandler _greetingHandler = new GreetingHandler();

        protected BotMessages Messages
        {
            get
            {
                return MessagesLazy.Value;
            }
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // TODO: move betting logic to BettingDialog

            if (_greetingHandler.IsCountingStarted(activity.Text))
            {
                await _teamCounter.InitPoll(activity, _greetingHandler.GetCountingChoices(activity.RemoveRecipientMention()));
            }
            else if (_greetingHandler.IsBetStarted(activity.Text))
            {
                ConversationStarter.Start(activity, () => new BettingDialog(AppData.BotMesasges));
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}