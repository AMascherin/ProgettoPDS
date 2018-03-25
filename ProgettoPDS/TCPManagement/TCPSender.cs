using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace ProgettoPDS
{
    class TCPSender
    {


        private int PORT_NO = 13370;
       // private static IPAddress targetAddress;
       // private IPEndPoint targetEndPoint;
      //  private string SERVER_IP = "127.0.0.1";
        private static TcpClient client;

        public TCPSender(string ip)
        {
            //   targetAddress = IPAddress.Parse(ip);
            //   targetEndPoint = new IPEndPoint(targetAddress, PORT_NO);
            //   client = new TcpClient(targetEndPoint);
            client = new TcpClient(ip, PORT_NO);
            System.Windows.MessageBox.Show("TCP Sender create");
        }


        public void SendMessage(Object obj)
        {

            try
            {
                //---create a NetworkStream---
                NetworkStream nwStream = client.GetStream();

                //Convert message string into byte data
                byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes((string)obj);

                //Send data into stream
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                nwStream.Close();
                CloseConnection();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }

        public void CloseConnection()
        {
            client.Close();
        }

        public void SendData(Object data)
         {

             Thread t = new Thread(new ParameterizedThreadStart(SendMessage));
             t.Start(data);
             //t.detach();

          }

    }
}

