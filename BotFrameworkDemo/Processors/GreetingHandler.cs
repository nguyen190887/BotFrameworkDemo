using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Processors
{
    [Serializable]
    public class GreetingHandler
    {
        const string CounterStartKeyword = "dem che";

        const string BettingStartKeyword = "bet";

        public bool IsCountingStarted(string message)
        {
            return message.StartsWith(CounterStartKeyword, StringComparison.OrdinalIgnoreCase);
        }

        public string[] GetCountingChoices(string message)
        {
            return message
                .Replace(CounterStartKeyword, string.Empty)
                .TrimStart(':')
                .Trim()
                .Split(',')
                .Select(x => x.Trim().ToUpper())
                .ToArray();
        }

        public bool IsBetStarted(string message)
        {
            return message.StartsWith(BettingStartKeyword, StringComparison.OrdinalIgnoreCase);
        }
    }
}