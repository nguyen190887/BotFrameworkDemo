﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BotFrameworkDemo.Dialogs;
using BotFrameworkDemo.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace BotFrameworkDemo
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<SandwichOrder> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                        // Actually process the sandwich order...
                        await context.PostAsync("Processed your order!");
                    }
                    catch (FormCanceledException<SandwichOrder> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last} -- maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit. Please try again.";
                        }
                        await context.PostAsync(reply);
                    }
                });
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                if (activity.RemoveBotMention() == "sandwich")
                {
                    await Conversation.SendAsync(activity, MakeRootDialog);
                }
                else
                {
                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static readonly object _memberLock = new object();
        private Activity HandleSystemMessage(Activity message)
        {
            string messageType = message.GetActivityType();
            if (messageType == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (messageType == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                lock (_memberLock)
                {
                    HashSet<string> memberSet = AppData.ChannelMemberCounter.ContainsKey(message.ChannelId) ? AppData.ChannelMemberCounter[message.ChannelId] : new HashSet<string>();
                    if (message.MembersAdded != null && message.MembersAdded.Count > 0)
                    {
                        //memberCount += message.MembersAdded.Count;
                        foreach (var member in message.MembersAdded)
                        {
                            memberSet.Add(member.Id);
                        }
                    }
                    if (message.MembersRemoved != null && message.MembersRemoved.Count > 0)
                    {
                        //memberCount -= message.MembersRemoved.Count;
                        foreach (var member in message.MembersRemoved)
                        {
                            memberSet.Remove(member.Id);
                        }
                    }
                    AppData.ChannelMemberCounter[message.ChannelId] = memberSet;
                }
            }
            else if (messageType == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (messageType == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (messageType == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}