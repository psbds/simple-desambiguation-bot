using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : BaseLuisScoreDialog<object>
    {
        // Frases que serão utilizadas para desambiguação
        private readonly IDictionary<string, string> DesambiguationTable = new Dictionary<string, string>() {
             {"create_account","I want to create an account" },
             {"ask_card","I want to request a card" }
        };

        private async Task GoToRoot(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                await result;
            }
            catch (Exception exc)
            {
                await context.PostAsync(exc.Message);
            }
            finally
            {
                context.Wait(MessageReceived);
            }
        }


        [LuisIntent("ask_card")]
        private async Task AskCard_Start(IDialogContext context, LuisResult result)
        {
            context.Call(new CardIntentDialog(result), GoToRoot);
        }

        [LuisIntent("create_account")]
        private async Task Create_AccountStart(IDialogContext context, LuisResult result)
        {
            context.Call(new CreateAccountDialog(result), GoToRoot);
        }

        // Confiança Média
        [LuisIntent(MEDIUM_THRESHOLD_DIALOG)]
        private async Task Desambiguation(IDialogContext context, LuisResult result)
        {
            List<CardAction> buttons = new List<CardAction>();

            for (var i = 0; i < result.Intents.Count && i < 2; i++)
            {
                var intentName = result.Intents[i].Intent;
                var canonical = DesambiguationTable.ContainsKey(intentName) ? DesambiguationTable[intentName] : null;
                if (canonical == null)
                    continue;
                CardAction cityBtn1 = new CardAction()
                {
                    Value = canonical,
                    Type = "postBack",
                    Title = canonical
                };
                buttons.Add(cityBtn1);
            }
            if (buttons.Any())
            {
                var reply = context.MakeMessage();
                reply.Type = "message";
                reply.Attachments = new List<Attachment>();

                HeroCard plCard = new HeroCard()
                {
                    Buttons = buttons
                };
                Attachment plAttachment = plCard.ToAttachment();
                reply.Attachments.Add(plAttachment);
                await context.PostAsync("Sorry, I could understand right, do you want to ask anthing below?");
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync($"Sorry, I didn't understand your question");
            }
            context.Wait(MessageReceived);
        }

        // Confiança Baixa
        [LuisIntent("None")]
        [LuisIntent(LOW_THRESHOLD_DIALOG)]
        private async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry, I didn't understand your question");
            context.Wait(MessageReceived);
        }

    }
}