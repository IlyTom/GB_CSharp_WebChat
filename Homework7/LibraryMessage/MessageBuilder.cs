using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryMessage
{
    public class MessageBuilder
    {
        private Message _message = Message.GetInstance();

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

        public MessageBuilder SetIsRead(bool isRead)
        {
            _message.IsRead = isRead;
            return this;
        }

        public Message Build()
        {
            return _message;
        }
    }
}
