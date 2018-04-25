using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MessagesTransferApi.Logic
{
    class HttpSender
    {
        public async void SendPostRequest(HttpContent content, string url)
        {
            using (var client = new HttpClient())
            {
                var httpRequest = await client.PostAsync(url, content);
            }
        }
    }
}
