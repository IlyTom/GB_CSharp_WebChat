using NetMQ.Sockets;

namespace LibraryMessage
{
    public interface IMessageSource
    {
        void SendMessage(ResponseSocket server, Message msg);

        public Message ReceiveMessage(ResponseSocket server);
    }
}
