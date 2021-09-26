using Sport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerTCP
{
    class Client
    {
        class DataPacket
        {
            public NetworkStream Stream { get; set; }
            public byte[] Buffer { get; set; }

            public DataPacket(int length)
            {
                Buffer = new byte[length];
            }
        }

        private TcpClient c;
        private AsyncCallback callback;
        private NetworkStream stream;

        public int Key { private set; get; }

        public event Listener.OnClientDisconnect Disconnect;

        public Client(int key, TcpClient c)
        {
            this.c = c;
            Key = key;
            stream = c.GetStream();

            Data();
        }

        bool _waitForId = false;
        bool _saveObject = false;

        private void Data()
        {
            if (callback == null)
            {
                callback = new AsyncCallback(OnReceived);
            }
            DataPacket dPacket = new DataPacket(1000);
            dPacket.Stream = c.GetStream();
            stream.BeginRead(dPacket.Buffer, 0, dPacket.Buffer.Length, callback, dPacket);
        }

        public void OnReceived(IAsyncResult IAresult)
        {
            DataPacket dp = (DataPacket)IAresult.AsyncState;

            if (!c.Connected)
            {
                ConnectionDropped(dp.Stream);
                return;
            }

            int rx;

            try
            {
                rx = dp.Stream.EndRead(IAresult);
            }
            catch
            {
                ConnectionDropped(dp.Stream);
                return;
            }

            if (rx == 0)
            {
                ConnectionDropped(dp.Stream);
                return;
            }

            Payload(dp.Buffer);

            Data();
        }

        private void ConnectionDropped(NetworkStream stream)
        {
            Console.WriteLine($"Client: {c.Client.RemoteEndPoint} disconnected.");
            Disconnect(Key);
        }

        public void Send(byte[] data)
        {
            stream.WriteAsync(data, 0, data.Length);
        }

        public void Payload(byte[] buffer)
        {
            string message = Encoding.UTF8.GetString(buffer).TrimEnd(new char[] { (char)0 }).Trim().ToLower();

            if (!_waitForId && !_saveObject)
            {
                switch (message.ToLower())
                {
                    case "hentalle":
                        Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ObjectStore.Get())));
                        break;
                    case "hent":
                        _saveObject = false;
                        _waitForId = true;
                        break;
                    case "gem":
                        _saveObject = true;
                        _waitForId = false;
                        break;
                    default:
                        Send(Encoding.UTF8.GetBytes("Ukendt kommando\n"));
                        break;
                }
            }
            else if (_waitForId)
            {
                if (message.ToLower() == "q")
                {
                    _waitForId = false;
                }
                else
                {
                    int id;

                    try
                    {
                        id = Convert.ToInt32(message);

                        Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ObjectStore.Get(id))));

                        _waitForId = false;
                    }
                    catch
                    {
                        Send(Encoding.UTF8.GetBytes("ID skal være et tal\n"));
                    }
                }               
            }
            else
            {
                try
                {
                    ObjectStore.Players.Add(JsonSerializer.Deserialize<FootballPlayer>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
                    Send(Encoding.UTF8.GetBytes("Spiller gemt\n"));
                }
                catch
                {
                    Send(Encoding.UTF8.GetBytes("Fejl i JSON streng\n"));
                }

                _saveObject = false;
            }
        }
    }
}
