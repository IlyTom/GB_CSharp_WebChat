using Client;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<Message> messagesReceive = new List<Message>();
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var serverTask = StartServer(token, messagesReceive);
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

        static async Task StartServer(CancellationToken cts, List<Message> messagesReceive)
        {
            UdpClient server = new UdpClient(4555);
            while (!cts.IsCancellationRequested)
            {   
                           
                var remoteEP = await server.ReceiveAsync(cts);
                
                string jsonMsg = Encoding.UTF8.GetString(remoteEP.Buffer);
                if (!jsonMsg.ToLower().Equals("listen"))
                {
                    Message? message = Message.Deserialize(jsonMsg);
                    Console.WriteLine($"[{message.DateTime}] Получено сообшение: \"{message.Text}\" от {message.Sender}");
                    message.IsRead = false;
                    messagesReceive.Add(message);

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
                else
                {
                    string response = "";
                    for (int i = 0; i < messagesReceive.Count; i++)
                    {
                        if (i!= messagesReceive.Count - 1)
                        {
                            response += messagesReceive[i].Serialize() + ", ";
                        }
                        else 
                        {
                            response += messagesReceive[i].Serialize();
                        }
                    }
                    Byte[] dataResponse = Encoding.UTF8.GetBytes(response);
                    await server.SendAsync(dataResponse, dataResponse.Length, remoteEP.RemoteEndPoint);
                }                
            }
        }
        
    }
}