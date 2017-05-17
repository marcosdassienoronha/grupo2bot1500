using ChatBot.Serialization;
using ChatBot.Services;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using System.Web.Http;


//O que está aqui não é para ser deletado/alterado sem aviso prévio,
//apenas pode ser acrescentado novos códigos ao já existente.  
namespace ChatBot.Controllers
{
    public class MessagesController : ApiController
    {
        public async Task<Activity> Post([FromBody]Activity message)
        {
            var connector = new ConnectorClient(new Uri(message.ServiceUrl));

            //Método Start
            var resposta = await Start(message);
            var msg = message.CreateReply(resposta, "pt-BR");
            await connector.Conversations.SendToConversationAsync(msg);

            //Método Cases 
            var resposta2 = await Cases(message);
            var msg2 = message.CreateReply(resposta2, "pt-BR");
            await connector.Conversations.SendToConversationAsync(msg2);


            return await Task.FromResult<Activity>(msg2);

        }

        private static async Task<string> Start(Activity message)
        {
            Activity resposta = new Activity();
            var response = await Luis.GetResponse(message.Text);



            if (response != null)
            {
                resposta = message.CreateReply("Está de acordo com a negociação?");

            }

            return resposta.Text;
        }

        private static async Task<string> Cases(Activity message)
        {
            Activity resposta = new Activity();
            var response = await Luis.GetResponse(message.Text);

            if (response != null)
            {
                var intent = new Intent();
                var entity = new Serialization.Entity();

                string acao = string.Empty;
                string sim = string.Empty;
                string nao = string.Empty;
                string agendaResult = string.Empty;

                foreach (var item in response.entities)
                {
                    switch (item.type) //Resposta já vinculada ao LUIS, ou seja, detecta a intenção do usuario, se era SIM ou NAO que o mesmo queria dizer.
                    {
                        case "sim":
                            sim = item.entity;
                            break;
                        case "nao":
                            nao = item.entity;
                            break;
                    }
                }


                if (!string.IsNullOrEmpty(sim))
                    resposta = message.CreateReply($"Obrigado! Foi um prazer negociar com você. Abraços e até mais! ");
                else if (!string.IsNullOrEmpty(nao))
                    resposta = message.CreateReply("Tudo bem! Obrigado."); //Nesta primeira versão não será possível fazer uma contrapropósta (Cliente -> Banco)
                else
                    resposta = message.CreateReply("Não entendi o que você quis dizer.");

            }
            return resposta.Text;
        }

    }
}
