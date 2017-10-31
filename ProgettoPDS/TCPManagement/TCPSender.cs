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


            private int PORT_NO = 5000;
            private string SERVER_IP = "127.0.0.1";
            private TcpClient client;

            public void TcpSender(int port, string ip)
            {

                PORT_NO = port;
                SERVER_IP = ip;
                client = new TcpClient(SERVER_IP, PORT_NO);
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

            public void SendData(Object obj)
            {

                Thread t = new Thread(new ParameterizedThreadStart(Send));
                t.Start(data);
                t.detach();

            }

        }
    }
}
