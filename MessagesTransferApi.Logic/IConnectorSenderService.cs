using MessagesTransferApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesTransferApi.Logic
{
    public interface IConnectorSenderService
    {
        void SendAccountData(Connector connector, NetworkAuthData authData);
        void SendMessage(Connector connector, Message message, string AccessToken);
    }
}
