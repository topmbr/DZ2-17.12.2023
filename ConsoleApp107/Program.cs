using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace ConsoleApp107
{
    internal class Server
    {
        private UdpClient udpServer;
        private Dictionary<string, double> componentPrices;
        private Dictionary<string, int> clientRequestCount;
        private Dictionary<string, DateTime> lastClientActivity;
        private object lockObject = new object();

        public Server()
        {
            udpServer = new UdpClient(1234);
            componentPrices = new Dictionary<string, double>
        {
            {"processor", 200.0},
            {"memory", 50.0},
            {"storage", 100.0},
            {"graphics card", 300.0},
            // Додайте інші компоненти за необхідності
        };
            clientRequestCount = new Dictionary<string, int>();
            lastClientActivity = new Dictionary<string, DateTime>();
        }

        public void Start()
        {
            Console.WriteLine("Server started. Waiting for clients...");

            while (true)
            {
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpServer.Receive(ref clientEndpoint);

                string clientAddress = clientEndpoint.Address.ToString();
                string request = Encoding.ASCII.GetString(data);

                // Task 2: Check request limit
                if (!CheckRequestLimit(clientAddress))
                {
                    Console.WriteLine($"Request limit exceeded for client {clientAddress}");
                    continue;
                }

                // Task 3: Check and disconnect inactive clients
                CheckInactiveClients();

                // Process request
                string response = ProcessRequest(request);

                // Log client activity
                LogClientActivity(clientAddress, request);

                // Send response to the client
                udpServer.Send(Encoding.ASCII.GetBytes(response), response.Length, clientEndpoint);
            }
        }

        private bool CheckRequestLimit(string clientAddress)
        {
            lock (lockObject)
            {
                if (!clientRequestCount.ContainsKey(clientAddress))
                {
                    clientRequestCount[clientAddress] = 0;
                }

                clientRequestCount[clientAddress]++;

                return clientRequestCount[clientAddress] <= 10;
            }
        }

        private void CheckInactiveClients()
        {
            lock (lockObject)
            {
                DateTime currentTime = DateTime.Now;

                foreach (var kvp in lastClientActivity.ToArray())
                {
                    if ((currentTime - kvp.Value).TotalMinutes >= 10)
                    {
                        Console.WriteLine($"Client {kvp.Key} disconnected due to inactivity.");
                        lastClientActivity.Remove(kvp.Key);
                        clientRequestCount.Remove(kvp.Key);
                    }
                }
            }
        }

        private string ProcessRequest(string request)
        {
            string response;

            if (componentPrices.ContainsKey(request.ToLower()))
            {
                double price = componentPrices[request.ToLower()];
                response = $"Price for {request} is ${price}";
            }
            else
            {
                response = $"Component {request} not found";
            }

            return response;
        }

        private void LogClientActivity(string clientAddress, string request)
        {
            lock (lockObject)
            {
                if (!lastClientActivity.ContainsKey(clientAddress))
                {
                    lastClientActivity[clientAddress] = DateTime.Now;
                }
                else
                {
                    lastClientActivity[clientAddress] = DateTime.Now;
                }

                Console.WriteLine($"Logged: Client {clientAddress}, Request: {request}, Time: {DateTime.Now}");
            }
        }

        static void Main()
        {
            Server server = new Server();
            server.Start();
        }

    }
}