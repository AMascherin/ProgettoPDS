using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TestApplication
{
    class UDPSender
    {

        private static UdpClient udpclient;
        private static IPAddress groupAddress;
        private IPEndPoint Clientdest;
        private UserConfiguration user;

        private static char[] tGreetings = { 'H', 'e', 'l', 'l', 'o', ' ', 'O', 'r', 'i', 'g', 'i', 'n', 'a', 't', 'o', 'r', '!' };
        
        public UDPSender(string MulticastAddress)

        {
            user = new UserConfiguration();
            udpclient = new UdpClient();
            groupAddress = IPAddress.Parse(MulticastAddress);
            udpclient.JoinMulticastGroup(groupAddress);
            Clientdest = new IPEndPoint(groupAddress, 2000);
        }

        private static Byte[] GetByteArray(Char[] ChArray)

        {
            Byte[] Ret = new Byte[ChArray.Length];
            for (int i = 0; i < ChArray.Length; i++)
                Ret[i] = (Byte)ChArray[i];
            return Ret;
        }

        public void Start()
        {
            while (true)

            {
                Thread.Sleep(3000);
              //  lock (user._UserDatalocker) ;
                if (user.PrivacyFlag)
                    udpclient.Send(GetByteArray(tGreetings), tGreetings.Length, Clientdest);//TODO: Specificare dati da passare
                else break;

            }
        }



    }

}

