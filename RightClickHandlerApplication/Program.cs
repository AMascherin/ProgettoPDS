using System;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace RightClickHandlerApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(remoteEP);
                int i = 0;
                byte[] msg = Encoding.ASCII.GetBytes(args[0]);
                // Send the data through the socket.  
                int bytesSent = client.Send(msg);
                i++;

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
