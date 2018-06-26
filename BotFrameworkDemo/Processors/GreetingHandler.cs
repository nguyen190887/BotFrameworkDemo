using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Processors
{
    [Serializable]
    public class GreetingHandler
    {
        const string CounterStartKeyword = "đếm";

        const string BettingStartKeyword = "bet";

        public bool IsCountingStarted(string message)
        {
            return message.StartsWith(CounterStartKeyword, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsBetStarted(string message)
        {
            return message.StartsWith(BettingStartKeyword, StringComparison.OrdinalIgnoreCase);
        }
    }
}