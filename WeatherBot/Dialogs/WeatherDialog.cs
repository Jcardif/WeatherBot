using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;

namespace WeatherBot.Dialogs
{
    [Serializable]
    public class WeatherDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            
        }
    }
}