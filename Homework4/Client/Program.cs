using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await StartClient();
        }

        static async Task StartClient()
        {
            List<Message> messagesReceive = new List<Message>();
            bool isRunning = true;
            Console.WriteLine("Введите Ваш никнейм:");
            string nickname = Console.ReadLine()!;
            UdpClient udpClient = new UdpClient();
            udpClient.Connect("127.0.0.1", 4555);


            while (isRunning)
            {
                Console.WriteLine("Введите сообщение: ");
                string message = Console.ReadLine()!;
                if (message.ToLower().Equals("exit"))
                {
                    isRunning = false;
                    udpClient.Close();
                }
                else if (message.ToLower().Equals("listen"))
                {
                    Byte[] buffer = Encoding.UTF8.GetBytes("listen");
                    await udpClient.SendAsync(buffer);
                    await ReceiveAllHistoryMessage(udpClient, messagesReceive);
                }
                else
                {
                    Message msg = new MessageBuilder()
                        .SetText(message)
                        .SetDateTime(DateTime.Now)
                        .SetSender(nickname)
                        .SetReceiver("Server")
                        .Build();
                    string data = msg.Serialize();
                    Byte[] buffer = Encoding.UTF8.GetBytes(data);
                    await udpClient.SendAsync(buffer);
                    
                    await ReceiveMessage(udpClient);                    
                }
            }
        }
        
        static async Task ReceiveMessage(UdpClient udpClient)
        {
            var remoteEP = await udpClient.ReceiveAsync();            
            try
            {
                Byte[] bufferResponse = remoteEP.Buffer;
                string jsonResponse = Encoding.UTF8.GetString(bufferResponse);
                Message? response = Message.Deserialize(jsonResponse);
                Console.WriteLine($"[{DateTime.Now}] Ваше сообщение: \"{response.Text}\"");
            }
            catch (SocketException)
            {
                Console.WriteLine($"[{DateTime.Now}] Ваше сообщение не было доставлено!");
            }
        }

        static async Task ReceiveAllHistoryMessage(UdpClient udpClient, List<Message> messages)
        {
            var remoteEP = await udpClient.ReceiveAsync();
            try 
            {
                Byte[] bufferResponse = remoteEP.Buffer;
                string listMessage = Encoding.UTF8.GetString(bufferResponse);
                List<Message> messagesServer = JsonSerializer.Deserialize<List<Message>>(listMessage);
                foreach (var message in messagesServer)
                {
                    Console.WriteLine($"{message.Text} - {message.Sender} - {message.IsRead}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }
}