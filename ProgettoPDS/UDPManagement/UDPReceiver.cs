using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProgettoPDS
{
    class UDPReceiver
    {
        private const int listenPort = 2000;

        private static void StartListener()
        {
            bool done = false;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);
                    string data = Encoding.Unicode.GetString(bytes);
                    UserConfiguration user = JsonConvert.DeserializeObject<UserConfiguration>(data);
                    Console.WriteLine("Received broadcast from {0} :\n {1}\n", groupEP.ToString(), Encoding.Unicode.GetString(bytes, 0, bytes.Length));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
