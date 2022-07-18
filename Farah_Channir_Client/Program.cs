using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Farah_Channir_Client
{
    class Program
    {
       public static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to chat Client Server Program you Can Start with Chat ... ");
            Console.WriteLine("To Send the messages write xx ... ");


            using (ClientWebSocket client_WebSocket=new ClientWebSocket())
            {
                string messag = "";
                Uri Servruri = new Uri("ws://localhost:5000/send");
                var Cancellation_Token = new CancellationTokenSource();
                Cancellation_Token.CancelAfter(TimeSpan.FromSeconds(120));
                try
                {
                    await client_WebSocket.ConnectAsync(Servruri, Cancellation_Token.Token);
                   
                    while (client_WebSocket.State== WebSocketState.Open)
                    {

                       
                        var temp = "";
                        while (temp.ToLower() != "xx")
                        {

                            temp = Console.ReadLine();
                            if (temp.ToLower() != "xx")
                                messag += temp + Environment.NewLine;

                        }
                        if (!string.IsNullOrEmpty(messag))
                        {
                            ArraySegment<byte> bytst_send = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messag));
                            await client_WebSocket.SendAsync(bytst_send, WebSocketMessageType.Text, true, Cancellation_Token.Token);
                            var ResponceBufer = new byte[10240];
                            var offest = 0;
                            var Paket = 1024;
                            while (true)
                            {
                                ArraySegment<byte> byte_Rescive = new ArraySegment<byte>(ResponceBufer, offest, Paket);
                                WebSocketReceiveResult response_Client = await client_WebSocket.ReceiveAsync(byte_Rescive, Cancellation_Token.Token);
                                var RsponceMessage = Encoding.UTF8.GetString(ResponceBufer, offest, response_Client.Count);
                                Console.WriteLine(RsponceMessage);
                                if(response_Client.EndOfMessage)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception Exp)
                {

                    Console.WriteLine(Exp.Message);
                }

            }
       
            Console.ReadLine();

        }
    }
}
