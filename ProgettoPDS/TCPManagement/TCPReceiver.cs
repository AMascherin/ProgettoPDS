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
using System.Windows.Forms;
using ProgettoPDS.GUI;


//Todo:integrare con il mainhub
namespace ProgettoPDS
{
    public class handleClient
    {
        TcpClient clientSocket;
        private UserConfiguration uc = new UserConfiguration();
        //string clNo;
        public void startClient(TcpClient inClientSocket)
        {
            this.clientSocket = inClientSocket;
            Thread ctThread = new Thread(receiveFileCommunication);
            ctThread.Start();
        }
        private void receiveFileCommunication()
        {
            byte[] bytesFrom = new byte[10025]; //TODO:capire la dimensione
            string dataFromClient = null;
            Byte[] receivedData = null;
           // string serverResponse = null;
            //string rCount = null;

            while ((true))
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
                    Console.WriteLine( "From client-" + dataFromClient);

                    //Deconvertire il json
                    //Far comparire 
                    if (uc.AutomaticDownloadAcceptance == false)
                    {
                        RicezioneFile rcf = new RicezioneFile(new List<String>()); //La lista arriva dal file json decompresso
                        rcf.Show();
                        if (false) { //Risposta dall'interfaccia grafica
                            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes("418 I'm a teapot");
                            networkStream.Write(bytesToSend, 0, bytesToSend.Length); //Send to the server the file information data
                            networkStream.Flush();
                            networkStream.Close();
                            clientSocket.Close();
                        }
                    }
                    
                    //Ricezione dei dati e barra di avanzamento di download
                    


                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }

            }
        }
    }

        class TCPReceiver
    {


        private int PORT_NO = 13370;

        private static TcpListener listener;
        private static TcpClient clientSocket = default(TcpClient);

        private static Boolean done;

        public TCPReceiver()
        {
            IPAddress localAdd = IPAddress.Any;
            listener = new TcpListener(localAdd, PORT_NO);
            done = false;

        }

        public void StartListener() {


            listener.Start();
            Console.WriteLine("Listener started");
            while (!done) {
                clientSocket = listener.AcceptTcpClient();
                handleClient client = new handleClient();
                client.startClient(clientSocket);
            }

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

        void AskAcceptFiles() {

            MessageBox.Show("Do you want to receive files?", "",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

    }
}
