using System;
using System.Collections.Generic;
using System.Text;
using MessagesTransferApi.Data.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MessagesTransferApi.Logic
{
    public class DirectConnecterSenderService : IConnectorSenderService
    {
        private HttpSender sender = new HttpSender();

        public async void SendAccountData(Connector connector, NetworkAuthData authData)
        {
            var serializedMessage = await Task.Run(() => JsonConvert.SerializeObject(new
            {
                accessToken = authData.AccessToken
            }));

            var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");

            sender.SendPostRequest(content, connector.Url);
        }

        public async void SendMessage(Connector connector, Message message, string accessToken)
        {
            var serializedMessage = await Task.Run(() => JsonConvert.SerializeObject(new
            {
                accessToken = accessToken,
                senderId = message.NetworkUserId,
                text = message.Text
            }));

            var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");

            sender.SendPostRequest(content, connector.Url);
        }
    }
}
