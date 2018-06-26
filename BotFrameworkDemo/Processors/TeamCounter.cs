using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Processors
{
    [Serializable]
    public class TeamCounter
    {
        const string StartKeyword = "đếm";

        [NonSerialized]
        private Timer t;

        public bool IsCountingStarted(string message)
        {
            return message.StartsWith(StartKeyword, StringComparison.OrdinalIgnoreCase);
        }

        public async Task InitPoll(Activity activity)
        {
            var conversationReference = activity.ToConversationReference();
            ConversationStarter.conversationReference = JsonConvert.SerializeObject(conversationReference);


            t = new Timer(new TimerCallback(timerEvent));
            t.Change(2000, Timeout.Infinite);

            //var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            //IMessageActivity newMessage = Activity.CreateMessageActivity();
            //newMessage.Type = ActivityTypes.Message;
            //newMessage.Text = "Hello, on a new thread";
            //ConversationParameters conversationParams = new ConversationParameters(
            //    isGroup: true,
            //    bot: null,
            //    members: null,
            //    topicName: "Test Conversation",
            //    activity: (Activity)newMessage,
            //    channelData: activity.ChannelData);
            //return await connector.Conversations.CreateConversationAsync(conversationParams);
        }


        public void timerEvent(object target)
        {
            t.Dispose();
            ConversationStarter.Resume(); //We don't need to wait for this, just want to start the interruption here
        }
    }
}