using Autofac;
using BotFrameworkDemo.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
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
    public class ConversationStarter
    {
        ////Note: Of course you don't want these here. Eventually you will need to save these in some table
        ////Having them here as static variables means we can only remember one user :)
        //public static string fromId;
        //public static string fromName;
        //public static string toId;
        //public static string toName;
        //public static string serviceUrl;
        //public static string channelId;
        //public static string conversationId;

        ////This will send an adhoc message to the user
        //public static async Task Resume(string conversationId, string channelId)
        //{
        //    var userAccount = new ChannelAccount(toId, toName);
        //    var botAccount = new ChannelAccount(fromId, fromName);
        //    var connector = new ConnectorClient(new Uri(serviceUrl));

        //    IMessageActivity message = Activity.CreateMessageActivity();
        //    if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
        //    {
        //        message.ChannelId = channelId;
        //    }
        //    else
        //    {
        //        conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
        //    }
        //    message.From = botAccount;
        //    message.Recipient = userAccount;
        //    message.Conversation = new ConversationAccount(id: conversationId);
        //    message.Text = "Hello, this is a notification";
        //    message.Locale = "en-Us";
        //    await connector.Conversations.SendToConversationAsync((Activity)message);
        //}


        //Note: Of course you don't want this here. Eventually you will need to save this in some table
        //Having this here as static variable means we can only remember one user :)
        public static string conversationReference;

        //This will interrupt the conversation and send the user to SurveyDialog, then wait until that's done 
        public static async Task Resume()
        {
            var message = JsonConvert.DeserializeObject<ConversationReference>(conversationReference).GetPostToBotMessage();
            var client = new ConnectorClient(new Uri(message.ServiceUrl));

            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(CancellationToken.None);

                //This is our dialog stack
                var task = scope.Resolve<IDialogTask>();

                //interrupt the stack. This means that we're stopping whatever conversation that is currently happening with the user
                //Then adding this stack to run and once it's finished, we will be back to the original conversation
                var dialog = new SurveyDialog();
                task.Call(dialog.Void<object, IMessageActivity>(), null);

                await task.PollAsync(CancellationToken.None);

                //flush dialog stack
                await botData.FlushAsync(CancellationToken.None);

            }
        }
    }
}