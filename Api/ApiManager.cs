using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web.Http;
using Fleck;
using Microsoft.Owin.Hosting;
using OsuLoader.Api.Websocket;
using Owin;

namespace OsuLoader.Api;

public class ApiManager
{
    private static string address = "127.0.0.1";
    private static ushort apiPort = GetAvailablePort();
    private static ushort wsPort = GetAvailablePort();
    private static List<string> loadedRoutes = new();
    
    private static ushort GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return (ushort)port;
    }
    
    public static void Run()
    {
        var apiServer = new Thread(() =>
        {
            using (WebApp.Start<StartUp>($"http://{address}:{apiPort}/"))
            {
                Console.WriteLine($"Server started at http://{address}:{apiPort}/");
                Console.WriteLine("Loaded routes:");
                foreach (var route in loadedRoutes)
                {
                    Console.WriteLine($"\t\"{route}\"");
                }
                
                Thread.Sleep(Timeout.Infinite);
            }
        });
        apiServer.IsBackground = true;
        apiServer.Start();

        var websocketSever = new Thread(() =>
        {
            var server = new WebSocketServer($"ws://{address}:{wsPort}/");
            server.Start(socket =>
            {
                var controller = new WebsocketController(socket);
            });
            
            Thread.Sleep(Timeout.Infinite);
        });
        websocketSever.IsBackground = true;
        websocketSever.Start();
    }

    private class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}");
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            loadedRoutes.AddRange(config.Routes.Select(route => route.RouteTemplate));
            app.UseWebApi(config);
        }
    }
}