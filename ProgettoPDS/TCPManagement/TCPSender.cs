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
        private static IPAddress targetAddress;
        private IPEndPoint targetEndPoint;
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


        public void Send(Object obj)
        {
            

            //---create a TCPClient object at the IP and port no.---

            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend;


            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                bytesToSend = ms.ToArray();
            }

            //---send data---

            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

            //---read back the text---
            //byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            //int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            //Console.ReadLine();

        }

        public void CloseConnection()
        {

            client.Close();
        }

        public void SendData(Object data)
         {

             Thread t = new Thread(new ParameterizedThreadStart(Send));
             t.Start(data);
             //t.detach();

          }

    }
}

