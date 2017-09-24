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

        public UDPSender(string MulticastAddress)

        {
            user = new UserConfiguration();
            udpclient = new UdpClient();
            groupAddress = IPAddress.Parse(MulticastAddress);
            udpclient.JoinMulticastGroup(groupAddress);
            Clientdest = new IPEndPoint(groupAddress, 2000); //Definire port number
        }

        public void Start()
        {
            while (true)

            {
                Thread.Sleep(3000);
                if (user.PrivacyFlag)
                {
                    string data = user.GetJSONConfiguration();
                    udpclient.Send(Encoding.Unicode.GetBytes(data), data.Length, Clientdest);
                }
                else break;

            }
        }


    }
}
