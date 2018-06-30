using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    public class SampleScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public SampleScorable(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            // TODO
            throw new NotImplementedException();
        }

        protected override double GetScore(IActivity item, string state)
        {
            // TODO
            return 0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            // TODO
            return false;
        }

        protected override Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            throw new NotImplementedException();
            // TODO
        }

        protected override Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            throw new NotImplementedException();
            // TODO
        }
    }
}