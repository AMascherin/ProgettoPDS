using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace ProgettoPDS
{
    class UDPReceiver
    {
        private static BlockingCollection<ReceivedData> dataItems;
        private UserConfiguration user;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
        private struct ReceivedData
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
        {
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
            private byte[] data;
            public IPAddress senderIpAddr;
            public ReceivedData(byte[] data, IPAddress ipAddr) {
                this.data = data;
                senderIpAddr = ipAddr;
            }

            public static bool operator!= (ReceivedData op1, ReceivedData op2) {
                return !op1.Equals(op2);
            }
            public static bool operator ==(ReceivedData op1, ReceivedData op2)
            {
                return op1.Equals(op2);
            }

            public override string ToString()
            {
                return "Data: " + Encoding.Unicode.GetString(data)+ "\n IPAddr: " + senderIpAddr.ToString();
            }

            public string dataToString() {
                return Encoding.Unicode.GetString(data);
            }
        }
        

        public UDPReceiver() {
            dataItems= new BlockingCollection<ReceivedData>(64);
            user = new UserConfiguration();
        }
        public void StartListener()
        {           
            
            List<NetworkInterface> validNetwork = new List<NetworkInterface>();
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces()) {
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
           
            
            Thread processingThread = new Thread(ProcessData);
            processingThread.Name = "ProcessingThread";
            processingThread.Start();

            // client.Client.Bind(new IPEndPoint(IPAddress.Any, listenPort));
            foreach (NetworkInterface adapter in validNetwork) {
                

                IPInterfaceProperties p = adapter.GetIPProperties();
                foreach (var address in p.UnicastAddresses) {
                    if (address.Address.AddressFamily.Equals(AddressFamily.InterNetwork)&&!address.Address.Equals(IPAddress.Loopback)) {


                        UdpClient client = new UdpClient();
                        IPAddress multicastAddress = IPAddress.Parse(user.GetMulticastUDPAddress());
                        IPEndPoint localEndPoint = new IPEndPoint(address.Address, user.GetUDPPort());
                        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        client.MulticastLoopback = false;
                        client.ExclusiveAddressUse = false;
                        client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddress, address.Address));

                        client.Client.Bind(localEndPoint);

                        Thread receiverThread = new Thread(new ParameterizedThreadStart(ReceiveData));
                        receiverThread.Start(client);
                    }
                }      

            }
       } 

        private static void ReceiveData(Object obj) {
            UdpClient client = (UdpClient)obj;
            UserConfiguration user = new UserConfiguration();
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, user.GetUDPPort());
            try
            {
                bool done = false;
                while (!done)
                {
                //    Console.WriteLine("Waiting for broadcast");
                    byte[] data = new Byte[1024];
                    data = client.Receive(ref remoteIPEndPoint);
                    ReceivedData receivedData = new ReceivedData(data, remoteIPEndPoint.Address);
                    dataItems.Add(receivedData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                // listener.Close();
                dataItems.CompleteAdding();
            }
        }

        private static void ProcessData() {
            UserConfiguration uc = new UserConfiguration();
            while (!dataItems.IsCompleted) {

                //Byte[] receivedData = null;
                ReceivedData receivedData = new ReceivedData();
                try {
                    receivedData = dataItems.Take();
                }
                catch (InvalidOperationException) {

                    string message = "Invalid Operation. Please try again";
                    string caption = "Invalid operation";

                    DialogResult result;

                    // Displays the MessageBox.

                    result = MessageBox.Show(message, caption);


                }  // IOE means that Take() was called on a completed collection.
                
                if (receivedData != null)
                {
                    string data = receivedData.dataToString();
                    NetworkUser user = new NetworkUser(data);
                    user.Ipaddress = receivedData.senderIpAddr.ToString();
                    string localMAC = uc.GetMACAddress();
                    if (user.MACAddress != localMAC) //Scarta i pacchetti generati dallo stesso PC che li riceve
                        NetworkUserManager.AddUser(user);
                }
            }
        }

    }
}

