using MessagesTransferApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesTransferApi.Logic
{
    public interface IAggregatorSenderService
    {
        void SendMessage(User user, Message message);
    }
}
