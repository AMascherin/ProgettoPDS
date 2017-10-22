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
            while (!dataItems.IsCompleted) {
                System.Windows.MessageBox.Show("Dato elaborato");
                Byte[] receivedData = null;
                try {
                    receivedData = dataItems.Take();
                }
                catch (InvalidOperationException) { }  // IOE means that Take() was called on a completed collection.
                if (receivedData != null)
                {
                    string data = Encoding.Unicode.GetString(receivedData);
                    NetworkUser user = new NetworkUser(data); //TODO: Aggiungere il Network User alla lista degli utenti
                    //Scartare i pacchetti generati dallo stesso PC

                }
            }
        }

    }
}

