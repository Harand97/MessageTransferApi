using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesTransferApi.Data.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string NetworkName { get; set; }

        public string ReceiverId { get; set; }

        public string Text { get; set; }
    }
}
