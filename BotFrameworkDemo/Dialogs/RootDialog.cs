using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotFrameworkDemo.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.IndexOf(helloRequest, StringComparison.OrdinalIgnoreCase) > -1)
            {
                await context.PostAsync(helloResponse);
            }
            else if (activity.Text.IndexOf(startKeyword, StringComparison.OrdinalIgnoreCase) > -1 ||
                activity.Text.IndexOf(separator, StringComparison.OrdinalIgnoreCase) > -1)
            {
                await context.PostAsync(BetFor(activity.Text));
                await context.PostAsync(endMessage);
            }
            else
            {
                // Calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // Return our reply to the user
                await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            }
            context.Wait(MessageReceivedAsync);
        }

        const string helloRequest = "ê bot";
        const string helloResponse = "Gì vậy đại ca???";
        const string startKeyword = "bắt";
        const string endKeyword = "vậy";
        const string separator = "hay";
        const string finalMessageFormat = "Bắt {0} đi. Nghĩ sao mà đi bắt {1} vậy !!!";
        const string endMessage = "Đừng tin Mèo một nhoa !!!";
        const string errorMessage = "Không đoán được. Nhập 'bắt xxx hay yyy vậy?' đi !!!";

        private string BetFor(string message)
        {
            string trimmed = message.TrimEnd('?').RemoveStart(startKeyword).RemoveEnd(endKeyword);
            string[] teams = trimmed.Split(new[] { separator }, StringSplitOptions.None)
                .Select(x => x.Trim())
                .ToArray();

            if (teams.Length != 2)
            {
                return errorMessage;
            }

            int winnerIndex = GetTeamIndex();
            int loserIndex = 1 - winnerIndex;
            return string.Format(finalMessageFormat, teams[winnerIndex].ToUpper(), teams[loserIndex].ToUpper());
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
}