using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Net.NetworkInformation;


namespace ProgettoPDS
{
    class UDPSender
    {
        private UserConfiguration user;

        public UDPSender()
        {
            user = new UserConfiguration();
        }

        public void Start()
        {
            List<NetworkInterface> validNetwork = new List<NetworkInterface>();
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (!adapter.SupportsMulticast) //Multicast non supportato
                    continue;

                if (!adapter.GetIPProperties().MulticastAddresses.Any())
                    continue;

                if (!adapter.Supports(NetworkInterfaceComponent.IPv4))
                    continue;

                IPv4InterfaceProperties p = adapter.GetIPProperties().GetIPv4Properties();
                if (null == p)
                    continue; // IPv4 is not configured on this adapter

                if (adapter.OperationalStatus == OperationalStatus.Up)
                    validNetwork.Add(adapter);

            }


            foreach (NetworkInterface adapter in validNetwork)
            {
                IPInterfaceProperties p = adapter.GetIPProperties();
                foreach (var address in p.UnicastAddresses)
                {
                    if (address.Address.AddressFamily.Equals(AddressFamily.InterNetwork) && !address.Address.Equals(IPAddress.Loopback))
                    {
                        UdpClient client = new UdpClient();
                        IPAddress multicastAddress = IPAddress.Parse(user.GetMulticastUDPAddress());
                        IPEndPoint localEndPoint = new IPEndPoint(address.Address, 0);
                        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                        client.MulticastLoopback = false;
                        client.ExclusiveAddressUse = false;
                        client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddress, address.Address));

                        client.Client.Bind(localEndPoint);

                        Thread receiverThread = new Thread(new ParameterizedThreadStart(StartClient));
                        receiverThread.Start(client);
                    }
                }
            }
        }            

        public void StartClient(Object obj)
        {
            UdpClient udpclient = (UdpClient)obj;
            IPAddress multicastAddress = IPAddress.Parse(user.GetMulticastUDPAddress());
            IPEndPoint remoteEndPoint = new IPEndPoint(multicastAddress, user.GetUDPPort());
            while (true)
            {
                if (user.PrivacyFlag)
                {

                    try
                    {
                        string data = user.GetJSONConfiguration();
                        Byte[] data_b = Encoding.Unicode.GetBytes(data);
                        udpclient.Send(data_b, data_b.Length, remoteEndPoint);
                    }
                    catch (ArgumentNullException ex)
                    {
                        break;

                    }
                    catch (EncoderFallbackException ex)
                    {
                        break;

                    }
                }
                else break;
                Thread.Sleep(10000);

            }
        }
    }
}
