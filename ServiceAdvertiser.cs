using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Makaretu.Dns;
using System.Net.Sockets;
using System.Net;
using System;
using Fleck;

namespace ValRestServer
{
    public class WebSocket
    {
        private readonly ConcurrentDictionary<IWebSocketConnection, object> connections = new ConcurrentDictionary<IWebSocketConnection, object>();
        public async Task StartServer()
        {
            // Advertise the server using mDNS.
            var serviceInfo = new ServiceProfile("Player Name", "_http._tcp", 8765);

            var mdns = new ServiceDiscovery();

            mdns.Advertise(serviceInfo);
            Console.WriteLine("Advertised websocket server");

            WebStart();
            Console.WriteLine("WebSocket Server started");
        }
        public void WebStart()
        {
            var server = new WebSocketServer("ws://0.0.0.0:8765");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("WebSocket connection opened.");
                    connections.TryAdd(socket, null);
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine("WebSocket connection closed.");
                    connections.TryRemove(socket, out _);
                };

                socket.OnMessage = message =>
                {
                    Console.WriteLine("Received message: " + message);
                    BroadcastMessage(message);
                };
            });
        }

        public void BroadcastMessage(string message)
        {
            var socketConnections = connections.Keys.ToList();
            foreach (var connection in socketConnections)
            {
                connection.Send(message);
            }
        }


    }
        
}