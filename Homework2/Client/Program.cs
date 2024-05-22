using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
                else
                {
                    Message msg = new Message(message, DateTime.Now, nickname, "Server");
                    string data = msg.Serialize();
                    Byte[] buffer = Encoding.UTF8.GetBytes(data);
                    udpClient.Send(buffer);
                    Thread receiveThread = new Thread(() => ReceiveMessage(udpClient));
                    receiveThread.Start();;                  
                }
            }
        }
        
        static void ReceiveMessage(UdpClient udpClient)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 4555);            
            try
            {
                Byte[] bufferResponse = udpClient.Receive(ref remoteEP);
                string jsonResponse = Encoding.UTF8.GetString(bufferResponse);
                Message? response = Message.Deserialize(jsonResponse);
                Console.WriteLine($"[{DateTime.Now}] Ваше сообщение: \"{response.Text}\"");
            }
            catch (SocketException)
            {
                Console.WriteLine($"[{DateTime.Now}] Ваше сообщение не было доставлено!");
            }
        }
    }
}