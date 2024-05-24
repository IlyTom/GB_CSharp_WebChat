using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client
{
    internal class Message
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public Message(string Text, DateTime DateTime, string Sender, string Receiver) { 
            this.Text = Text;
            this.DateTime = DateTime;
            this.Sender = Sender;
            this.Receiver = Receiver;
        }

        public string Serialize() => JsonSerializer.Serialize(this);

        public static Message? Deserialize(string message) => JsonSerializer.Deserialize<Message>(message);

    }
}
