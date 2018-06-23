﻿using System;
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

            if (greetingRequests.ContainsText(activity.Text))
            {
                await context.PostAsync(greetingMessages.PickOne());
            }
            else if (activity.Text.IndexOf(startKeyword, StringComparison.OrdinalIgnoreCase) > -1 ||
                activity.Text.IndexOf(separator, StringComparison.OrdinalIgnoreCase) > -1)
            {
                await context.PostAsync(BetFor(activity.Text));
                await context.PostAsync(finalMessages.PickOne());
            }
            else
            {
                await context.PostAsync(notUnderstands.PickOne());
                await context.PostAsync(unpredictableMessage);
            }
            context.Wait(MessageReceivedAsync);
        }

        //const string helloRequest = "ê bot";
        //const string helloResponse = "Gì vậy đại ca???";
        const string startKeyword = "bắt";
        const string endKeyword = "vậy";
        const string separator = "hay";
        //const string finalMessageFormat = "Bắt {0} đi. Nghĩ sao mà đi bắt {1} vậy !!!";
        const string unpredictableMessage = "Không đoán được. Nhập 'bắt xxx hay yyy vậy?' đi !!!";

        private string[] greetingRequests = new[]
        {
            "e bot",
            "ê bot",
            "hello",
            "bot ơi",
            "chào"
        };

        private string[] greetingMessages = new[]
        {
            "Gì vậy đại ca???",
            "Đang ngủ mà... Có gì không?",
            "Dạ, có em",
            "Nghe nè!"
        };

        private string[] notUnderstands = new[]
        {
            "Chat gì khó vậy? Bot demo mà :D",
            "Tào lao",
            "Không hiểu gì cả!!!",
            "Đang suy nghĩ ...",
            "Hiểu chết liền !!!"
        };

        private string[] finalMessages = new[]
        {
            "Đừng tin mèo Một nhoa !!!",
            "Tin tui đi, thua thì chửi tui nè :D",
            "Liều ăn nhiều. Tiền thôi mà !!!",
            "Làm tới đi pa :))"
        };

        private string[] bettingMessages = new[]
        {
            "Bắt {0} đi. Nghĩ sao mà đi bắt {1} vậy !!!",
            "Cứ {0} mà bắt. {1} đá tệ lắm",
            "Đừng có theo {1}, {0} nó mạnh lắm !",
            "Theo {0} đi"
        };

        private string BetFor(string message)
        {
            string trimmed = message.TrimEnd('?').RemoveStart(startKeyword).RemoveEnd(endKeyword);
            string[] teams = trimmed.Split(new[] { separator }, StringSplitOptions.None)
                .Select(x => x.Trim())
                .ToArray();

            if (teams.Length != 2)
            {
                return unpredictableMessage;
            }

            int winnerIndex = GetTeamIndex();
            int loserIndex = 1 - winnerIndex;
            return bettingMessages.PickOneWithParams(teams[winnerIndex].ToUpper(), teams[loserIndex].ToUpper());
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

        public static bool ContainsText(this IEnumerable<string> collection, string text)
        {
            return collection.Any(x => text.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1);
        }
    }
}