using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Farah_Channir_ChatServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
            app.UseWebSockets(wsOptions);
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/send")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                         
                            await Send(context, webSocket);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            });
        }
        private async Task Send(HttpContext httpContext, WebSocket web_Socket)
        {
            Console.WriteLine("To Send the messages write xx ... ");
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult resultFromReceve = await web_Socket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
            if (resultFromReceve != null)
            {
                while (!resultFromReceve.CloseStatus.HasValue)
                {

                    string MsgOfCliennt = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer));
                    Console.WriteLine($"Client: {MsgOfCliennt}");
                    var Message = "";
                    var temp = "";
                    Console.CursorVisible = true;
                    while (temp.ToLower() != "xx")
                    {
                        temp = Console.ReadLine();
                        Console.CursorVisible = true;

                        if (temp.ToLower() != "xx")
                            Message += temp + Environment.NewLine;
                       

                    }
                    await web_Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Server : " + Message)), resultFromReceve.MessageType, resultFromReceve.EndOfMessage, System.Threading.CancellationToken.None);
                    resultFromReceve = await web_Socket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);

                }



                await web_Socket.CloseAsync(resultFromReceve.CloseStatus.Value, resultFromReceve.CloseStatusDescription, System.Threading.CancellationToken.None);
            }
        }


    }
}
