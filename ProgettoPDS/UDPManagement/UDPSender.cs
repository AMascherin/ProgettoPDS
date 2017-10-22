using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ProgettoPDS
{
    class UDPSender
    {
        private static UdpClient udpclient;
        private static IPAddress groupAddress;
        private IPEndPoint Clientdest;
        private UserConfiguration user;

        public UDPSender() 

        {
            user = new UserConfiguration();
            udpclient = new UdpClient();
            groupAddress=IPAddress.Parse("255.255.255.255");  //IPv4 Broadcast
            Clientdest = new IPEndPoint(groupAddress, 13370); //Hard-Coded Port Number. TODO: Check confirmation user authorization
        }

        public void Start()
        {
            while (true)
            {                
                if (user.PrivacyFlag)
                {
                    string data = user.GetJSONConfiguration();
                    Byte[] data_b = Encoding.Unicode.GetBytes(data);
                    udpclient.Send(data_b, data_b.Length, Clientdest);
                }
                else break;
                Thread.Sleep(10000);

            }
        }

        public void SingleStart() {
            string data = user.GetJSONConfiguration();
            udpclient.Send(Encoding.Unicode.GetBytes(data), data.Length, Clientdest);
        }
    }
}
