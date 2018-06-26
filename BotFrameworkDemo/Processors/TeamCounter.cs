using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Processors
{
    [Serializable]
    public class TeamCounter
    {
        const string StartKeyword = "đếm";

        public bool IsCountingStarted(string message)
        {
            return message.StartsWith(StartKeyword, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ConversationResourceResponse> InitPoll(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            IMessageActivity newMessage = Activity.CreateMessageActivity();
            newMessage.Type = ActivityTypes.Message;
            newMessage.Text = "Hello, on a new thread";
            ConversationParameters conversationParams = new ConversationParameters(
                isGroup: true,
                bot: null,
                members: null,
                topicName: "Test Conversation",
                activity: (Activity)newMessage,
                channelData: activity.ChannelData);
            return await connector.Conversations.CreateConversationAsync(conversationParams);
        }
    }
}