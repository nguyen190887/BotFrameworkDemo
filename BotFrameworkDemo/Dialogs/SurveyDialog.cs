﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    #region Without prompt
    //[Serializable]
    //public class SurveyDialog : IDialog<object>
    //{
    //    public async Task StartAsync(IDialogContext context)
    //    {
    //        await context.PostAsync("Hello, I'm the survey dialog. I'm interrupting your conversation to ask you a question. Type \"done\" to resume");

    //        context.Wait(this.MessageReceivedAsync);
    //    }

    //    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
    //    {
    //        var activity = await result as Activity;

    //        var withoutMentions = GetTextWithoutMentions(activity);
    //        await context.PostAsync($"Before: {activity.Text} - After: {withoutMentions}");

    //        if (withoutMentions == "done")
    //        {
    //            await context.PostAsync("Great, back to the original conversation!");
    //            context.Done(String.Empty); //Finish this dialog
    //        }
    //        else
    //        {
    //            await context.PostAsync("I'm still on the survey until you type \"done\"");
    //            context.Wait(MessageReceivedAsync); //Not done yet
    //        }
    //    }

    //    private string GetTextWithoutMentions(Activity activity)
    //    {
    //        string output = activity.Text;

    //        // remove mentions
    //        foreach (var mention in activity.GetMentions())
    //        {
    //            output = output.Replace(mention.Text, "");
    //        }

    //        // remove @botname
    //        output = output.Replace($"@{activity.Recipient.Name}", "");

    //        return output.Trim();
    //    }
    //} 
    #endregion

    [Serializable]
    public class SurveyDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "Stay in this survey", "Get back to where I was" }, "Hello, you're in the survey dialog. Please pick one of these options");
        }

        private async Task AfterSelectOption(IDialogContext context, IAwaitable<string> result)
        {

            if ((await result) == "Get back to where I was")
            {
                await context.PostAsync("Great, back to the original conversation!");
                context.Done(String.Empty); //Finish this dialog
            }
            else
            {
                await context.PostAsync("I'm still on the survey until you tell me to stop");
                PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "Stay in this survey", "Get back to where I was" }, "Hello, you're in the survey dialog. Please pick one of these options");
            }
        }
    }
}