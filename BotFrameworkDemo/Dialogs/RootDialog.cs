using BotFrameworkDemo.Extensions;
using BotFrameworkDemo.Models;
using BotFrameworkDemo.Processors;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private TeamCounter _teamCounter = new TeamCounter();

        private GreetingHandler _greetingHandler = new GreetingHandler();

        private BetProcessor _betProcessor = new BetProcessor { Messages = AppData.BotMesasges };

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // TODO: move betting logic to BettingDialog
            var message = activity.RemoveBotMention();
            if (_greetingHandler.IsCountingStarted(message))
            {
                //await _teamCounter.InitPoll(activity, _greetingHandler.GetCountingChoices(text));
                //await context.Forward(
                //    new SurveyDialog(_greetingHandler.GetCountingChoices(message)), ResumeAfterSurvey, message, CancellationToken.None);
            }
            else if (message == "test")
            {
                // TODO: currently not working, need to research more
                context.Call(ChainDialogDemo.Simple(), ResumeAfterSurvey);
            }
            else
            //if (_greetingHandler.IsBetStarted(text))
            {
                //ConversationStarter.Start(activity, () => new BettingDialog(AppData.BotMesasges));
                await _betProcessor.ProcessBet(context, activity);
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterSurvey(IDialogContext context, IAwaitable<object> result)
        {

        }
    }
}