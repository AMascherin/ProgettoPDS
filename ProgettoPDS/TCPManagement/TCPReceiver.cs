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
using System.Runtime.Serialization;
using System.Diagnostics;

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

        //TODO: Inserire sistema per l'utente di rifiutare la ricezione del file

        public void ReceiveData()
        {
            //TODO: Gestire il path di ricezione secondo le scelte dell'utente
            //TODO: Gestione dei conflitti nel caso il file esista già
            

            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); 
            //TODO: Sostituire questo campo con il path scelto dall'utente nel file di configurazione o tramite apposita UI

            /*********************************************************************/
            
            string fileName = currentfolder+@"\test\Pippo.jpg";
            //TODO: Scegliere il nome e l'estensione in base alla comunicazione TCP svolta in precedenza


            /*********************************************************************/

            int counter = 0;
            //---listen at the specified IP and port no.---
            Console.WriteLine("Listening...");
            listener.Start(); //TODO: Spostare il listener in un altro metodo

            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Connection Accepted...");

            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();

            // Check to see if this NetworkStream is readable.
            if (nwStream.CanRead)
            {
                byte[] myReadBuffer = new byte[1024];
                
                int numberOfBytesRead = 0;
               
                // Incoming message may be larger than the buffer size.
                do
                {
                    
                    numberOfBytesRead = nwStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    Console.WriteLine("Bytes Received : " + numberOfBytesRead);
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(fileName, true))
                        {
                            writer.BaseStream.Write(myReadBuffer, 0, numberOfBytesRead);
                            counter++;
                        }
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine("DirectoryNotFoundException: {0}", e);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("ArgumentException: {0}", e);
                    }

                }
                while (nwStream.DataAvailable);
 
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            }

            Console.WriteLine(counter);
        }

        public void ReceiveMessage()
        {
            //---listen at the specified IP and port no.---
            Console.WriteLine("Listening...");
            listener.Start(); //TODO: Spostare il listener in un altro metodo

            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Connection Accepted...");

            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---read incoming stream---
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            Console.WriteLine("Bytes Received : " + bytesRead);

            try
            {                
                String dataReceived = String.Empty;
                dataReceived = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received : " + dataReceived);

            }

            catch (Exception e) {
                throw;
            }
            finally
            {
                client.Close();
            }


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

        public void Receive()
        {

            Thread t = new Thread(ReceiveMessage);
            t.Name = "TCPClient";
            t.Start();
            t.Join();


        }

    }
}
