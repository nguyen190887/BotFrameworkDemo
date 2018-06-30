using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Extensions
{
    public static class ActivityExtensions
    {
        public static string RemoveBotMention(this Activity activity)
        {
            string botName = activity.Recipient.Name;
            if (activity.Text.StartsWith(botName, StringComparison.OrdinalIgnoreCase))
            {
                return activity.Text.Substring(botName.Length).Trim();
            }
            return activity.Text;
        }
    }
}