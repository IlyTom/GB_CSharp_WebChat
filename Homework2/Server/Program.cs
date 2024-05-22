using Client;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.Start();                      
            Console.WriteLine("Нажмите любую кнопку для завершения работы сервера...");          
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void StartServer()
        {
            UdpClient server = new UdpClient(4555);
            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 4555);
                Byte[] data = server.Receive(ref remoteEP);
                string jsonMsg = Encoding.UTF8.GetString(data);
                Message? message = Message.Deserialize(jsonMsg);
                Console.WriteLine($"[{message.DateTime}] Получено сообшение: \"{message.Text}\" от {message.Sender}");

                //Response
                Message response = new Message($"Ваше сообщение: {message.Text} доставлено {message.Receiver}", DateTime.Now, "Server", message.Sender);
                Byte[] dateResponse = Encoding.UTF8.GetBytes(response.Serialize());
                server.Send(dateResponse, dateResponse.Length, remoteEP);
            }
        }
    }
}