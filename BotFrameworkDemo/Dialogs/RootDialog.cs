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

            if (_teamCounter.IsCountingStarted(activity.Text))
            {
                await _teamCounter.InitPoll(activity);
            }
            else if (Messages.GreetingRequests.PartialContains(activity.Text))
            {
                await context.PostAsync(Messages.GreetingMessages.PickOne());
            }
            else if (Messages.BettingKeyword.Starts.PartialContains(activity.Text) ||
                Messages.BettingKeyword.Separators.PartialContains(activity.Text))
            {
                await context.PostAsync(BetFor(activity.Text));
                await context.PostAsync(Messages.FinalMessages.PickOne());
            }
            else
            {
                await context.PostAsync(Messages.NotUnderstands.PickOne());
                await context.PostAsync(Messages.UnpredictableMessage);
            }
            context.Wait(MessageReceivedAsync);
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

    public static class TextExtensions
    {
        public static string RemoveStart(this string text, string search)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int startIndex = text.IndexOf(search, StringComparison.OrdinalIgnoreCase);
            if (startIndex >= 0)
            {
                return text.Substring(startIndex + search.Length).Trim();
            }
            return text;
        }

        public static string RemoveEnd(this string text, string search)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int endIndex = text.IndexOf(search, StringComparison.OrdinalIgnoreCase);
            if (endIndex >= 0)
            {
                return text.Substring(0, endIndex).Trim();
            }
            return text;
        }
    }

    public static class CollectionExtensions
    {
        public static T PickOne<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return default(T);
            }

            var ran = new Random(Guid.NewGuid().GetHashCode());
            var index = ran.Next(0, collection.Count() - 1);
            return collection.ElementAt(index);
        }

        public static string PickOneWithParams<T>(this IEnumerable<T> collection, params string[] args)
        {
            string format = collection.PickOne() as string;
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            return string.Format(format, args);
        }

        public static bool PartialContains(this IEnumerable<string> collection, string text)
        {
            return collection.Any(x => text.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> collection, string text)
        {
            return collection.Any(x => text.Equals(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}