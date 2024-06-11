using NetMQ;
using NetMQ.Sockets;
using LibraryMessage;
using System.ServiceModel.Channels;
using System.Net.Sockets;

namespace Server
{
    internal class Server : IMessageSource
    {
        private bool IsRunning;
        CancellationTokenSource cts;
        CancellationToken token;

        public Server()
        {
            IsRunning = true;
            cts = new CancellationTokenSource();
            token = cts.Token;
        }

        public LibraryMessage.Message ReceiveMessage(ResponseSocket server)
        {
            try
            {
                var message = server.ReceiveFrameString();
                LibraryMessage.Message? msg = LibraryMessage.Message.Deserialize(message);
                Console.WriteLine($"[{msg.DateTime}] Получено сообшение: \"{msg.Text}\" от {msg.Sender}");
                return msg;
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public void SendMessage(ResponseSocket server, LibraryMessage.Message message)
        {
            LibraryMessage.Message response = new MessageBuilder()
            .SetText($"Ваше сообщение: {message.Text} доставлено {message.Receiver}")
            .SetDateTime(DateTime.Now)
            .SetSender("Server")
            .SetReceiver(message.Sender)
            .Build();
            server.SendFrame(response.Serialize());
        }

        public void RunServer()
        {
            using (var server = new ResponseSocket())
            {
                Thread serverStart = new Thread(() =>
                {
                    server.Bind("tcp://localhost:4555");
                    Console.WriteLine("Server start...");
                    while (IsRunning)
                    {
                        try
                        {
                            var message = ReceiveMessage(server);

                            Thread.Sleep(100);

                            SendMessage(server, message);
                        }
                        catch (SocketException)
                        {
                            break;
                        }                        
                    }
                });
                Thread serverStop = new Thread(() =>
                {
                    try
                    {
                        Console.WriteLine("Нажмите любую кнопку для завершения работы сервера...");
                        Console.ReadKey();
                        cts.Cancel();
                        if (cts.IsCancellationRequested)
                        {
                            throw new OperationCanceledException();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Server closed.");
                        server.Close();
                    }
                });

                serverStart.Start();
                Thread.Sleep(1000);
                serverStop.Start();
            }
        }
    }
}
