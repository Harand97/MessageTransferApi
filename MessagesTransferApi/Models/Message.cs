using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesTransferApi.Models
{
    public class Message
    {
        public string NetworkName { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
    }
}
