﻿using Microsoft.Bot.Builder.ConnectorEx;
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
    [Serializable]
    public class TeamCounter
    {

        [NonSerialized]
        private Timer t;

        public async Task InitPoll(Activity activity)
        {
            var conversationReference = activity.ToConversationReference();
            ConversationStarter.conversationReference = JsonConvert.SerializeObject(conversationReference);


            t = new Timer(new TimerCallback(timerEvent));
            t.Change(2000, Timeout.Infinite);
        }


        public void timerEvent(object target)
        {
            t.Dispose();
            ConversationStarter.Resume(() => new SurveyDialog()); //We don't need to wait for this, just want to start the interruption here
        }
    }
}