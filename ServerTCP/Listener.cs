using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCP
{
    class Listener
    {
        Dictionary<int, Client> clients = new Dictionary<int, Client>();

        private TcpListener connection;

        Random rand = new Random();

        public delegate void OnClientDisconnect(int key);
        public delegate void OnClientConnect();

        public void Start(IPAddress ip, ushort port)
        {
            IPEndPoint endpoint;

            endpoint = new IPEndPoint(ip, port);

            connection = new TcpListener(endpoint);

            connection.Start();

            connection.BeginAcceptTcpClient(new AsyncCallback(OnConnecting), null);
        }

        /// <summary>
        /// Called when client connects to server
        /// </summary>
        /// <param name="IAresult"></param>
        private void OnConnecting(IAsyncResult IAresult)
        {
            TcpClient c = connection.EndAcceptTcpClient(IAresult);
            connection.BeginAcceptTcpClient(new AsyncCallback(OnConnecting), null);

            OnConnected(c);

            int key = rand.Next();
            lock (clients)
            {
                if (!clients.ContainsKey(key))
                {
                    Client client = new Client(key, c);
                    client.Disconnect += OnDisconnect;
                    clients.Add(key, client);
                }
            }
        }

        /// <summary>
        /// Called when client succesfully connected to server
        /// </summary>
        /// <param name="c"></param>
        private void OnConnected(TcpClient c)
        {
            Console.WriteLine($"Client: {c.Client.RemoteEndPoint.ToString()} connected.");
        }

        /// <summary>
        /// Called when a client has closed the connection
        /// </summary>
        /// <param name="key"></param>
        private void OnDisconnect(int key)
        {
            lock (clients)
            {
                clients.Remove(key);
            }
        }
    }
}
