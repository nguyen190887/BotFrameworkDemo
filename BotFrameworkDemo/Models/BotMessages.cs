using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Models
{
    [Serializable]
    public class BotMessages
    {
        public string[] GreetingRequests { get; set; }

        public string[] GreetingMessages { get; set; }

        public string[] NotUnderstands { get; set; }

        public string[] BettingMessages { get; set; }

        public string[] FinalMessages { get; set; }

        public string UnpredictableMessage { get; set; }

        public BettingKeyword BettingKeyword { get; set; }
    }

    [Serializable]
    public class BettingKeyword
    {
        public string[] Starts { get; set; }

        public string[] Ends { get; set; }

        public string[] Separators { get; set; }
    }
}