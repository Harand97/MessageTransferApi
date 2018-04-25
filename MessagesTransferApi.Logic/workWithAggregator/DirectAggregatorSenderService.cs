using System;
using System.Collections.Generic;
using System.Text;
using MessagesTransferApi.Data.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MessagesTransferApi.Logic
{
    public class DirectAggregatorSenderService : IAggregatorSenderService
    {
        private HttpSender sender = new HttpSender();

        public async void SendMessage(User user, Message message)
        {
            var serializedMessage = await Task.Run(() => JsonConvert.SerializeObject(new
            {
                userToken = user.UserToken,
                senderId = message.NetworkUserId,
                text = message.Text
            }));

            var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");

            sender.SendPostRequest(content, user.FeedbackUrl);
        }
    }
}
