using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    [LuisModel(
        "xxxxxxxxxxxxxxxxxxx",
        "xxxxxxxxxxxxxxxxxxx",
        LuisApiVersion.V2,
        domain: "westus.api.cognitive.microsoft.com",
        verbose: true,
        spellCheck: true)]
    public class BaseLuisScoreDialog<T> : LuisDialog<T>
    {
        public const String MEDIUM_THRESHOLD_DIALOG = "MEDIUM_THRESHOLD_DIALOG";
        public const String LOW_THRESHOLD_DIALOG = "LOW_THRESHOLD_DIALOG";

        protected double HighThreshold = 0.8;
        protected double LowThreshold = 0.4;

        protected Func<IDialogContext, LuisResult, Task> GoToHandler;

        protected override async Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestIntent, LuisResult result)
        {
            if (GoToHandler != null)
            {
                var task = GoToHandler(context, result);
                GoToHandler = null;
                await task;
            }
            else
            {
                if (bestIntent.Score < this.HighThreshold && bestIntent.Score > LowThreshold)
                {
                    bestIntent = new IntentRecommendation() { Intent = MEDIUM_THRESHOLD_DIALOG, Score = 1 };
                }
                else if (bestIntent.Score < this.LowThreshold)
                {
                    bestIntent = new IntentRecommendation() { Intent = LOW_THRESHOLD_DIALOG, Score = 1 };
                }
                await base.DispatchToIntentHandler(context, item, bestIntent, result);
            }

        }
    }
}