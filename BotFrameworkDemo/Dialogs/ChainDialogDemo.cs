using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    public class ChainDialogDemo
    {
        public static IDialog<string> Simple()
        {
            var query = from x in new PromptDialog.PromptString("prompt 1", "p1", 1)
                        from y in new PromptDialog.PromptString("prompt 2", "p2", 1)
                        select string.Join(" ", x, y);
            return query.PostToUser();
        }
    }
}