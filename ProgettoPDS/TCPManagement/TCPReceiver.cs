﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ProgettoPDS.GUI;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Diagnostics;

namespace ProgettoPDS
{
    class TCPServer
    {
        private int PORT_NO = 13370;

        private static TcpListener listener;

        private static Boolean done;

        public TCPServer()
        {
            listener = new TcpListener(IPAddress.Any, PORT_NO);
            done = false;

        }

        public void StartListener()
        {

            listener.Start();
            Console.WriteLine("Listener started");
            while (!done)
            {
                TcpClient clientSocket = listener.AcceptTcpClient();
                Console.WriteLine("New connection accepted");
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

                System.Windows.MessageBox.Show(clientRequest);
                networkStream.Flush();
               

                string DownloadPath = null;
                List<Models.DownloadItemModel> downloadItems = new List<Models.DownloadItemModel>(); //Deconversione JSON

                if (uc.AutomaticDownloadAcceptance == false) //Bisogna chiedere il permesso dall'utente
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        RicezioneFile rcf = new RicezioneFile(new List<String>()); //La lista arriva dal file json decompresso
                        rcf.ShowDialog();

                        if (!rcf.AcceptDownload) //Risposta dall'interfaccia grafica
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
                    });
                }

                else {
                    if (!uc.DefaultDownloadPath)
                    {
                        //TODO: Mostare la schermata per scegliere il path di download (deafult windows)
                    }
                    else
                        DownloadPath = uc.DefaultDownloadPathString;
                }
                //Avvisiamo il client che accettiamo la ricezione dei file
                byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes("200 OK");
                networkStream.Write(bytesToSend, 0, bytesToSend.Length);
                System.Windows.MessageBox.Show("200 Ok Send");
                networkStream.Flush();
                try
                {

                    //Ora è possibile ricevere i file
                    //string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    foreach (var downloadItem in downloadItems)
                    {
                        int count = 1;
                        string fileNameOnly = Path.GetFileNameWithoutExtension(DownloadPath);
                        string extension = Path.GetExtension(DownloadPath);
                        string directory = Path.GetDirectoryName(DownloadPath);
                        string newFullPath = DownloadPath;
                        while (File.Exists(DownloadPath))
                        {
                            string tmpFileName = string.Format("{0}({1})", fileNameOnly, count++);
                            newFullPath = Path.Combine(directory, tmpFileName + extension);
                        }


                        //string fileName = "name.png"; //TODO: ottenerla e gestire conflitti                
                        lock (this)
                        {
                            Stream fileStream = File.OpenWrite(DownloadPath); //TODO: Check if not null
                            byte[] clientData = new byte[chunkSize];
                            int bytesread = networkStream.Read(clientData, 0, clientData.Length); //Leggo il messaggio dell'utente
                            while (bytesread > 0)
                            {
                                fileStream.Write(clientData, 0, bytesread);
                                bytesread = networkStream.Read(clientData, 0, clientData.Length);
                            }
                            fileStream.Close();
                            System.Windows.MessageBox.Show("File data received, start save");  //TODO: Gestire la chiusura della connessione
                        }

                        //To extract the zip in a new folder
                        //ZipFile.ExtractToDirectory("zipname", "foldername");

                        /* Extracts in a target path
                        using (ZipFile zip = ZipFile.Read(file))
                        {
                            foreach (ZipEntry zipFiles in zip)
                            {
                                zipFiles.Extract(currentpath, true);
                            }

                        }*/

                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.StackTrace.ToString());
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