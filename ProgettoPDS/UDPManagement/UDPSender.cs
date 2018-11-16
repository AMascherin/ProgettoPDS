using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;


namespace ProgettoPDS
{
    class UDPSenderManager
    {
        private UserConfiguration user;
        private List<SenderData> senderThreads;
        public bool senderStatus;

        private struct SenderData
        {
            public Thread SenderThread;
            public UDPSender SenderObj;
        }

        public UDPSenderManager()
        {
            user = new UserConfiguration();
            senderThreads = new List<SenderData>();
            senderStatus = false;
        }
        public void Stop() {
            if (senderStatus)
            {
                senderStatus = false;
                foreach (var sender in senderThreads)
                {
                    sender.SenderObj.TerminateThread = true;
                    sender.SenderThread.Join();
                }
                senderThreads.Clear();
            }
        }

        public void Start()
        {
            if (!senderStatus)
            {
                senderStatus = true;

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

                            var senderData = new SenderData();
                            senderData.SenderObj = new UDPSender();
                            senderData.SenderThread = new Thread(new ParameterizedThreadStart(senderData.SenderObj.StartClient));                            
                            senderData.SenderThread.Start(client);
                            senderThreads.Add(senderData);
                        }
                    }

                }
            }
        }         
    }

    public class UDPSender
    { 
        public volatile bool TerminateThread = false;

        public void StartClient(object obj)
        {
            UserConfiguration user = new UserConfiguration();
            UdpClient udpclient = (UdpClient)obj;
            IPAddress multicastAddress = IPAddress.Parse(user.GetMulticastUDPAddress());
            IPEndPoint remoteEndPoint = new IPEndPoint(multicastAddress, user.GetUDPPort());
            
            while (!TerminateThread)
            {
              //  if (user.PrivacyFlag)
              //  {
                    try
                    {
                        string data = user.GetJSONConfiguration();
                        Byte[] data_b = Encoding.Unicode.GetBytes(data);
                        udpclient.Send(data_b, data_b.Length, remoteEndPoint);
                        Console.WriteLine("data sent");
                    }
                    catch (ArgumentNullException)
                    {
                        break;

                    }
                    catch (EncoderFallbackException)
                    {
                        break;

                    }
                    catch (SocketException)
                    {
                    break;
                    }
              //  }
              //  else break;
                Thread.Sleep(10000);

            }
            Console.WriteLine("Thread Stopped");
        }


    }
}
