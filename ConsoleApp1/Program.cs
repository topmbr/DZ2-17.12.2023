using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace ConsoleApp1
{
    internal class Client
    {
        private UdpClient udpClient;

        public Client()
        {
            udpClient = new UdpClient();
        }

        public void SendRequest(string request)
        {
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

            byte[] data = Encoding.ASCII.GetBytes(request);
            udpClient.Send(data, data.Length, serverEndpoint);

            byte[] receivedData = udpClient.Receive(ref serverEndpoint);
            string response = Encoding.ASCII.GetString(receivedData);

            Console.WriteLine($"Server response: {response}");
        }

        static void Main()
        {
            Client client = new Client();

            while (true)
            {
                Console.Write("Enter a computer component to get the price (e.g., processor): ");
                string request = Console.ReadLine();

                client.SendRequest(request);
            }
        }


    }
}