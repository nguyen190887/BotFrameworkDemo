using BotFrameworkDemo.Extensions;
using BotFrameworkDemo.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotFrameworkDemo.Processors
{
    [Serializable]
    public class BetProcessor
    {
        public BotMessages Messages { get; set; }

        public async Task ProcessBet(IDialogContext context, Activity activity)
        {
            var text = activity.RemoveBotMention();

            if (Messages.BettingKeyword.Starts.PartialContains(text) ||
                Messages.BettingKeyword.Separators.PartialContains(text))
            {
                await context.PostAsync(BetFor(text));
                await context.PostAsync(Messages.FinalMessages.PickOne());
                //context.Done(string.Empty);
            }
            else
            {
                await context.PostAsync(Messages.NotUnderstands.PickOne());
                await context.PostAsync(Messages.UnpredictableMessage);
                //context.Wait(MessageReceivedAsync);
            }
            //context.Done(string.Empty);
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