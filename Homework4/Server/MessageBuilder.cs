using System;
using Client;

namespace Server
{
    internal class MessageBuilder
    {
        private Message _message = new Message();

        public MessageBuilder SetSender(string sender)
        {
            _message.Sender = sender;
            return this;
        }
        public MessageBuilder SetReceiver(string receiver)
        {
            _message.Receiver = receiver;
            return this;
        }
        public MessageBuilder SetText(string text)
        {
            _message.Text = text;
            return this;
        }
        public MessageBuilder SetDateTime(DateTime dateTime)
        {
            _message.DateTime = dateTime;
            return this;
        }

        public Message Build()
        {
            return _message;
        }

    }
}
