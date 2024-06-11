using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryMessage
{
    public interface IMessageSourceClient
    {
        void SendMessage(RequestSocket client, string nickname);
        void ReceiveMessage(RequestSocket client);
    }
}
