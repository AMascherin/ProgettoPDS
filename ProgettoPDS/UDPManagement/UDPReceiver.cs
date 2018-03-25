using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace ProgettoPDS
{
    class UDPReceiver
    {
        private const int listenPort = 13370;
        private static BlockingCollection<Byte[]> dataItems;
        

        public UDPReceiver() {
            dataItems= new BlockingCollection<byte[]>(64);
        }
        public void StartListener()
        {
            bool done = false;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Broadcast, 0);
            Thread processingThread = new Thread(ProcessData);
            processingThread.Name = "ProcessingThread";
            processingThread.Start();
            
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    Byte[] data = listener.Receive(ref remoteIPEndPoint);
                    dataItems.Add(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
                dataItems.CompleteAdding();
            }
        }

        private static void ProcessData() {
            UserConfiguration uc = new UserConfiguration();
            NetworkUserManager manager = new NetworkUserManager();
            while (!dataItems.IsCompleted) {
                
                Byte[] receivedData = null;
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
                    string data = Encoding.Unicode.GetString(receivedData);
                    System.Windows.MessageBox.Show(data);
                    NetworkUser user = new NetworkUser(data); 
                    string localMAC = uc.GetMACAddress();
                    if (user.MACAddress != localMAC) //Scarta i pacchetti generati dallo stesso PC che li riceve
                        manager.AddUser(user);
                }
            }
        }

    }
}

