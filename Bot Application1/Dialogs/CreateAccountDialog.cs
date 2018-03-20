using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class CreateAccountDialog : BaseLuisScoreDialog<object>
    {
        [NonSerialized] // NonSerializable Necessário
        private readonly LuisResult _entranceResult;

        // Recebe a analise da mensagem que ativou o dialogo
        public CreateAccountDialog(LuisResult entranceResult)
        {
            _entranceResult = entranceResult;
        }

        // Inicia o Dialog utilizando a mensagem que ativou o dialogo
        public override async Task StartAsync(IDialogContext context)
        {
            context.ConversationData.SetValue("counter", 1);
            await this.RetrieveEntities(context, this._entranceResult);
        }

        private async Task RetrieveEntities(IDialogContext context, LuisResult result)
        {
            // Tratamento para que o usuário não fique preso em um loop caso não dê a resposta
            if (context.ConversationData.GetValue<int>("counter") > 3)
            {
                context.Fail(new Exception("Sorry, I didn't understand what you said"));
                return;
            }

            context.ConversationData.SetValue<int>("counter", context.ConversationData.GetValue<int>("counter") + 1);

            var creditCardTypes = result.Entities.Where(e => e.Type == "account_type").ToList();

            if (creditCardTypes.Count() == 0)
            {
                await context.PostAsync($"Do you want a Single or Joint Account?");
                this.GoToHandler = RetrieveEntities;
                context.Wait(MessageReceived);
            }
            else if (creditCardTypes.Count() == 1)
            {
                var cardType = ((JArray)creditCardTypes.FirstOrDefault().Resolution.FirstOrDefault().Value).FirstOrDefault().Value<String>();
                if (cardType == "joint")
                {
                    await context.PostAsync($"To ask for joint account please access: http://simplebotjointaccountlink");
                }
                else
                {
                    await context.PostAsync($"To ask for single accout please access: http://simplebosingleaccountlink");
                }
                context.Done("OK");
            }
            else
            {
                await context.PostAsync($"You must inform only one account Type");
                await context.PostAsync($"Do you want a Single or Joint Account?");
                this.GoToHandler = RetrieveEntities;
                context.Wait(MessageReceived);
            }
        }

    }
}