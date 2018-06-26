using BotFrameworkDemo.Extensions;
using BotFrameworkDemo.Models;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    [Serializable]
    public class BettingDialog : IDialog<object>
    {
        public BotMessages Messages { get; set; }

        public BettingDialog(BotMessages messages)
        {
            Messages = messages;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Messages.GreetingMessages.PickOne());

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;

            if (Messages.BettingKeyword.Starts.PartialContains(activity.Text) ||
                Messages.BettingKeyword.Separators.PartialContains(activity.Text))
            {
                await context.PostAsync(BetFor(activity.Text));
                await context.PostAsync(Messages.FinalMessages.PickOne());
                context.Done(string.Empty);
            }
            else
            {
                await context.PostAsync(Messages.NotUnderstands.PickOne());
                await context.PostAsync(Messages.UnpredictableMessage);
                context.Wait(MessageReceivedAsync);
            }
        }

        private string BetFor(string message)
        {
            string trimmed = message.TrimEnd('?');

            trimmed = RemoveStartAndEndKeyword(trimmed);

            string[] teams = trimmed.Split(Messages.BettingKeyword.Separators, StringSplitOptions.None)
                .Select(x => x.Trim())
                .ToArray();

            if (teams.Length != 2)
            {
                return Messages.UnpredictableMessage;
            }

            int winnerIndex = GetTeamIndex();
            int loserIndex = 1 - winnerIndex;
            return Messages.BettingMessages.PickOneWithParams(teams[winnerIndex].ToUpper(), teams[loserIndex].ToUpper());
        }


        private string RemoveStartAndEndKeyword(string trimmed)
        {
            foreach (var start in Messages.BettingKeyword.Starts)
            {
                string removed = trimmed.RemoveStart(start);
                if (removed != trimmed)
                {
                    trimmed = removed;
                    break;
                }
            }

            foreach (var end in Messages.BettingKeyword.Ends)
            {
                string removed = trimmed.RemoveEnd(end);
                if (removed != trimmed)
                {
                    trimmed = removed;
                    break;
                }
            }

            return trimmed;
        }

        private static int GetTeamIndex()
        {
            int seed = Guid.NewGuid().GetHashCode();
            var random = new Random(seed);
            var value = random.Next(1000000);
            return value % 2;
        }
    }
}
