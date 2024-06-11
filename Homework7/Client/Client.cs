using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryMessage;


namespace Client
{
    internal class Client : IMessageSourceClient
    {
        private bool _running;

        public Client()
        {
            _running = true;
        }

        public void ReceiveMessage(RequestSocket client)
        {
            try
            {
                var jsonResponse = client.ReceiveFrameString();
                Message? message = Message.Deserialize(jsonResponse);
                Console.WriteLine($"[{DateTime.Now}] Ваше сообщение: \"{message.Text}\"");
            }
            catch(FiniteStateMachineException)
            {
                Console.WriteLine("Клиент закрыт.");
                Thread.Sleep(5000);
            }            
        }

        public void SendMessage(RequestSocket client, string nickname)
        {
            Console.WriteLine("Введите сообщение: ");
            string message = Console.ReadLine()!;
            if (message!.ToLower().Equals("exit"))
            {
                _running = false;
                client.Close();
            }
            else
            {
                Message msg = new MessageBuilder()
                    .SetText(message)
                    .SetDateTime(DateTime.Now)
                    .SetSender(nickname)
                    .SetReceiver("Server")
                    .Build();
                client.SendFrame(msg.Serialize());                    
            }            
        }

        public void StartClient()
        {
            using (var client = new RequestSocket())
            {
                client.Connect("tcp://localhost:4555");
                Console.WriteLine("Введите Ваш никнейм: ");
                string nickname = Console.ReadLine()!;

                while(_running)
                {
                    SendMessage(client, nickname);
                    ReceiveMessage(client);
                }                
            }            
        }
    }
}
