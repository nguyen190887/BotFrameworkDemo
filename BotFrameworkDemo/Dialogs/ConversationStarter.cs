using Autofac;
using BotFrameworkDemo.Dialogs;
using Microsoft.Bot.Builder.ConnectorEx;
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

namespace BotFrameworkDemo.Dialogs
{
    public class ConversationStarter
    {
        //Note: Of course you don't want this here. Eventually you will need to save this in some table
        //Having this here as static variable means we can only remember one user :)
        public static string conversationReference;

        private static Timer _timer;

        public static void Start(Activity activity, Func<IDialog<object>> createDialog, int timeout = 2000)
        {
            var conversationReference = activity.ToConversationReference();
            ConversationStarter.conversationReference = JsonConvert.SerializeObject(conversationReference);


            _timer = new Timer(new TimerCallback(timerEvent), createDialog, 0, 0);
            _timer.Change(timeout, Timeout.Infinite);
        }

        public static void timerEvent(object target)
        {
            _timer.Dispose();

            var createDialog = target as Func<IDialog<object>>;
            Resume(createDialog); //We don't need to wait for this, just want to start the interruption here
        }

        //This will interrupt the conversation and send the user to SurveyDialog, then wait until that's done 
        public static async Task Resume(Func<IDialog<object>> createDialog)
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
                var dialog = createDialog();
                task.Call(dialog.Void<object, IMessageActivity>(), null);

                await task.PollAsync(CancellationToken.None);

                //flush dialog stack
                await botData.FlushAsync(CancellationToken.None);

            }
        }
    }
}