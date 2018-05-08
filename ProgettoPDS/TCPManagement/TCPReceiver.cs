using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ProgettoPDS.GUI;
using System.IO;

namespace ProgettoPDS
{
    class TCPServer
    {
        private int PORT_NO = 13370;

        private static TcpListener listener;

        private static Boolean done;

        public TCPServer()
        {
            IPAddress localAdd = IPAddress.Any;
            listener = new TcpListener(localAdd, PORT_NO);
            done = false;

        }

        public void StartListener()
        {

            listener.Start();
            Console.WriteLine("Listener started");
            while (!done)
            {
                TcpClient clientSocket = listener.AcceptTcpClient();
                handleClient client = new handleClient(clientSocket); //Nuovo thread per gestire la comunicazione con lo specifico utente
            }
        }

        public void CloseConnection()
        {
            listener.Stop();
        }
    }

    public class handleClient
    {
        private TcpClient clientSocket;
        private UserConfiguration uc;

        public handleClient(TcpClient inClientSocket)
        {
            uc = new UserConfiguration();
            this.clientSocket = inClientSocket;
            Thread ctThread = new Thread(handleFileTransfer);
            ctThread.Start();
        }

        private void handleFileTransfer()
        {
            int chunkSize = 2048;
            try
            {
                NetworkStream networkStream = clientSocket.GetStream(); //Apro un network stream con il client
                byte[] clientMessage = new byte[chunkSize];                 
                int bytesmessageread = networkStream.Read(clientMessage, 0, clientMessage.Length); //Leggo il messaggio dell'utente
                String clientRequest = System.Text.Encoding.UTF8.GetString(clientMessage);

                //Deconversione JSON

                if (uc.AutomaticDownloadAcceptance == false) //Bisogna chiedere il permesso dall'utente
                {
                    RicezioneFile rcf = new RicezioneFile(new List<String>()); //La lista arriva dal file json decompresso
                    rcf.Show();
                    if (false) //Risposta dall'interfaccia grafica
                    { 
                        //Se l'utente rifiuta si avvisa il sender e si chiude la connessione
                        networkStream.Write(System.Text.Encoding.UTF8.GetBytes("418 I'm a teapot"), 
                                            0, 
                                            System.Text.Encoding.UTF8.GetBytes("418 I'm a teapot").Length); //Send to the server the file information data
                        networkStream.Flush();
                        networkStream.Close();
                        clientSocket.Close();
                        return;
                    }        
                }
                //Avvisiamo il client che accettiamo la ricezione dei file
                byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes("200 OK");
                networkStream.Write(bytesToSend, 0, bytesToSend.Length);

                //Ora è possibile ricevere i file
                string folderPath = @"c:\"; //TODO: inserire o far scegliere quella dell'utente
                string fileName = "name"; //TODO: ottenerla e gestire conflitti                
                lock (this) {
                    Stream fileStream = File.OpenWrite(folderPath + fileName);
                    byte[] clientData = new byte[chunkSize];
                    int bytesread = networkStream.Read(clientData, 0, clientData.Length); //Leggo il messaggio dell'utente
                    while (bytesread > 0)
                    {
                        fileStream.Write(clientData, 0, bytesread);
                        bytesread = networkStream.Read(clientData, 0, clientData.Length);
                    }
                    fileStream.Close();
                }
                networkStream.Flush();
                networkStream.Close();
                clientSocket.Close();

            }
            catch (Exception ex) {
                Console.WriteLine(" >> " + ex.ToString());
            }
        }
    }
}


//Da rimuovere in successivi update:

//public void ReceiveData()
//{
//    //TODO: Gestire il path di ricezione secondo le scelte dell'utente
//    //TODO: Gestione dei conflitti nel caso il file esista già


//    string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
//    //TODO: Sostituire questo campo con il path scelto dall'utente nel file di configurazione o tramite apposita UI

//    /*********************************************************************/

//    string fileName = currentfolder + @"\test\Pippo.jpg";
//    //TODO: Scegliere il nome e l'estensione in base alla comunicazione TCP svolta in precedenza


//    /*********************************************************************/

//    int counter = 0;
//    //---listen at the specified IP and port no.---
//    Console.WriteLine("Listening...");
//    listener.Start(); //TODO: Spostare il listener in un altro metodo

//    //---incoming client connected---
//    TcpClient client = listener.AcceptTcpClient();
//    Console.WriteLine("Connection Accepted...");

//    //---get the incoming data through a network stream---
//    NetworkStream nwStream = client.GetStream();

//    // Check to see if this NetworkStream is readable.
//    if (nwStream.CanRead)
//    {
//        byte[] myReadBuffer = new byte[1024];

//        int numberOfBytesRead = 0;

//        // Incoming message may be larger than the buffer size.
//        do
//        {

//            numberOfBytesRead = nwStream.Read(myReadBuffer, 0, myReadBuffer.Length);
//            Console.WriteLine("Bytes Received : " + numberOfBytesRead);
//            try
//            {
//                using (StreamWriter writer = new StreamWriter(fileName, true))
//                {
//                    writer.BaseStream.Write(myReadBuffer, 0, numberOfBytesRead);
//                    counter++;
//                }
//            }
//            catch (DirectoryNotFoundException e)
//            {
//                Console.WriteLine("DirectoryNotFoundException: {0}", e);
//            }
//            catch (ArgumentException e)
//            {
//                Console.WriteLine("ArgumentException: {0}", e);
//            }

//        }
//        while (nwStream.DataAvailable);

//    }
//    else
//    {
//        Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
//    }

//    Console.WriteLine(counter);
//}

//public void ReceiveMessage()
//{
//    //---listen at the specified IP and port no.---
//    Console.WriteLine("Listening...");
//    listener.Start(); //TODO: Spostare il listener in un altro metodo

//    //---incoming client connected---
//    TcpClient client = listener.AcceptTcpClient();
//    Console.WriteLine("Connection Accepted...");

//    //---get the incoming data through a network stream---
//    NetworkStream nwStream = client.GetStream();
//    byte[] buffer = new byte[client.ReceiveBufferSize];

//    //---read incoming stream---
//    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

//    Console.WriteLine("Bytes Received : " + bytesRead);

//    try
//    {
//        String dataReceived = String.Empty;
//        dataReceived = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
//        Console.WriteLine("Received : " + dataReceived);

//    }

//    catch (Exception e)
//    {
//        throw;
//    }
//    finally
//    {
//        client.Close();
//    }


//    //---write back the text to the client---
//    // Console.WriteLine("Sending back : " + dataReceived);
//    // nwStream.Write(buffer, 0, bytesRead);


//    client.Close();
//    //Console.ReadLine();

//}
