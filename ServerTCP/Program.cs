using System;
using System.Net;
using System.Threading;

namespace ServerTCP
{
    class Program
    {
        static Listener listerner = new Listener();

        static void Main(string[] args)
        {
            listerner.Start(IPAddress.Any, 2121);

            while (true) // Keep it going ad infinitum
            {
                Thread.Sleep(5); // Don't eat all cpu. Thanks.
            }
        }
    }
}
