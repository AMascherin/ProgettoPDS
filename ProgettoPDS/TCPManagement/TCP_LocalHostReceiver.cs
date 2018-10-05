using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProgettoPDS
{
    class TCP_LocalHostReceiver
    {
        public TCP_LocalHostReceiver() { }

        public void StartListener() {
            //Ascolta il windows socket per l'invio dei file
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            string data = null;
            byte[] bytes = new Byte[1024];
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Waiting for connection ... ");
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    int bytesRec = handler.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                        
                        SchermataInvio invio = new SchermataInvio(data, NetworkUserManager.userlist);
                        invio.Show();

                    });

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
