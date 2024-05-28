using Client;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var serverTask = StartServer(token);
            Console.WriteLine("Нажмите любую кнопку для завершения работы сервера...");
            Console.ReadKey();
            cts.Cancel();  
            
            try
            {             
                await serverTask;                
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сервер остановлен.");
            }
        }

        static async Task StartServer(CancellationToken cts)
        {
            UdpClient server = new UdpClient(4555);
            while (!cts.IsCancellationRequested)
            {   
                           
                var remoteEP = await server.ReceiveAsync(cts);
                
                string jsonMsg = Encoding.UTF8.GetString(remoteEP.Buffer);
                Message? message = Message.Deserialize(jsonMsg);
                Console.WriteLine($"[{message.DateTime}] Получено сообшение: \"{message.Text}\" от {message.Sender}");

                //Response
                Message response = new MessageBuilder()
                        .SetText($"Ваше сообщение: {message.Text} доставлено {message.Receiver}")
                        .SetDateTime(DateTime.Now)
                        .SetSender("Server")
                        .SetReceiver(message.Sender)
                        .Build();
                Byte[] dateResponse = Encoding.UTF8.GetBytes(response.Serialize());
                await server.SendAsync(dateResponse, dateResponse.Length, remoteEP.RemoteEndPoint);

                
            }
        }
        
    }
}