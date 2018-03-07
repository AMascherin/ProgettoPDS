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
    class TCPReceiver
    {


        private int PORT_NO = 13370;

        private TcpListener listener;

        public TCPReceiver()
        {
            IPAddress localAdd = IPAddress.Any;
            listener = new TcpListener(localAdd, PORT_NO);

        }


        public void Receive()
        {
            //---listen at the specified IP and port no.---
            Console.WriteLine("Listening...");
            listener.Start();

            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Connection Accepted...");

            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---read incoming stream---
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            Console.WriteLine("Bytes Received : " + bytesRead);
            Console.WriteLine("Prova Sincro : " + bytesRead);

            //---convert the data received into a string---
            string dataReceived = System.Text.Encoding.Default.GetString(buffer);
            Console.WriteLine("Received : " + dataReceived);
            

            //---write back the text to the client---
            // Console.WriteLine("Sending back : " + dataReceived);
            // nwStream.Write(buffer, 0, bytesRead);

            client.Close();
            //Console.ReadLine();

        }

        public void CloseConnection()
        {
            listener.Stop();
        }

        public void ReceiveData()
        {

            Thread t = new Thread(Receive);
            t.Name = "TCPClient";
            t.Start();
            t.Join();


        }

    }
}
