﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ProgettoPDS.GUI;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProgettoPDS
{
    class TcpReceiver
    {

        private static TcpListener listener;

        private static Boolean done;

        private UserConfiguration uc = new UserConfiguration();

        public TcpReceiver()
        {
            listener = new TcpListener(IPAddress.Any, uc.GetTCPPort());
            done = false;

        }

        public void StartListener()
        {

            listener.Start();
            while (!done)
            {
                TcpClient clientSocket = listener.AcceptTcpClient();
                Console.WriteLine("New connection accepted");
                handleClient client = new handleClient(clientSocket); //Nuovo thread per gestire la comunicazione con lo specifico utente
            }
        }

        public void StopListener()
        {
            listener.Stop();
        }
    }

    public class handleClient
    {
        private TcpClient clientSocket;
        private UserConfiguration uc;
        private bool closeConnection = false;
        
        public handleClient(TcpClient inClientSocket)
        {
            uc = new UserConfiguration();
            this.clientSocket = inClientSocket;
            Thread ctThread = new Thread(handleFileTransfer);
            ctThread.Name="Handle Thread";
            ctThread.Start();
        }

        private void handleFileTransfer()
        {
            try
            {
                int chunkSize = 2048;
                NetworkStream networkStream = clientSocket.GetStream(); //Apro un network stream con il client
                byte[] clientMessage = new byte[chunkSize];
                int bytesmessageread = networkStream.Read(clientMessage, 0, clientMessage.Length); //Leggo il messaggio dell'utente
                String clientRequest = System.Text.Encoding.UTF8.GetString(clientMessage);

                networkStream.Flush();
                clientRequest = clientRequest.Replace("\0", string.Empty);

                if (clientRequest.Equals("Send Image"))
                {
                    byte[] imageByte = imageToByteArray(uc.ImgPath);
                    networkStream.Write(imageByte, 0, imageByte.Length);
                    Console.WriteLine("Image Sent");
                    CloseConnection();
                }

                else
                {
                    string DownloadPath = null;
                    List<Models.DownloadItemModel> downloadItems = new List<Models.DownloadItemModel>(); //Deconversione JSON

                    JObject json = JObject.Parse(clientRequest.ToString());

                    //DECONVERSIONE JSON
                    foreach (JProperty property in json.Properties())
                    {

                        JObject jobj = (JObject)property.Value;
                        Models.DownloadItemModel file = new Models.DownloadItemModel();
                        file.OriginalFileName = jobj.GetValue("nome").ToString();
                        file.Format = jobj.GetValue("estensione").ToString();
                        file.Dimension = (long)jobj.GetValue("dimensione");

                        Console.WriteLine(file.OriginalFileName + "+" + file.Format + "+" + file.Dimension.ToString());

                        downloadItems.Add(file);

                    }

                    if (uc.AutomaticDownloadAcceptance == false) //Bisogna chiedere il permesso dall'utente
                    {

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            RicezioneFile rcf = new RicezioneFile(downloadItems); //La lista arriva dal file json decompresso                            
                            rcf.ShowDialog();

                            if (!rcf.AcceptDownload) //Risposta dall'interfaccia grafica
                            {
                                //Se l'utente rifiuta si avvisa il sender e si chiude la connessione
                                //Send to the server the file information data
                                rcf.Reset();
                                networkStream.Write(System.Text.Encoding.UTF8.GetBytes("418 I'm a teapot"),
                                                     0,
                                                     System.Text.Encoding.UTF8.GetBytes("418 I'm a teapot").Length);

                                CloseConnection();
                            }
                            else
                            {
                                DownloadPath = rcf.downloadPath;
                                rcf.Reset();
                            }
                        });
                    }

                    else
                    {
                        if (!uc.DefaultDownloadPath)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                var dialog = new CommonOpenFileDialog();
                                dialog.IsFolderPicker = true;
                                CommonFileDialogResult result = dialog.ShowDialog();
                                if (result.ToString() == "Ok") {
                                    DownloadPath = Path.GetFullPath(dialog.FileName);
                                }
                            });

                        }
                        else
                            DownloadPath = uc.DefaultDownloadPathString;
                    }

                    if (closeConnection) return;
                    //Avvisiamo il client che accettiamo la ricezione dei file
                    byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes("200 OK");
                    networkStream.Write(bytesToSend, 0, bytesToSend.Length);
                    Console.WriteLine("200 Ok Send");
                    networkStream.Flush();

                    //Ora è possibile ricevere i file
                    foreach (var downloadItem in downloadItems)
                    {
                        string filePath = Path.Combine(DownloadPath, "tmp.zip");
                        int count = 1;
                        while (File.Exists(filePath))
                        {
                            string tmpFileName = string.Format("{0}({1})", "tmp", count++);
                            filePath = Path.Combine(DownloadPath, tmpFileName + ".zip");
                        }

                        lock (this)
                        {
                            Stream fileStream = File.OpenWrite(filePath); //TODO: Check if not null
                            byte[] clientData = new byte[chunkSize];
                            int bytesread = networkStream.Read(clientData, 0, clientData.Length); //Leggo il messaggio dell'utente
                            while (bytesread > 0)
                            {
                                fileStream.Write(clientData, 0, bytesread);
                                bytesread = networkStream.Read(clientData, 0, clientData.Length);
                            }
                            fileStream.Close();

                        }

                        try
                        {
                            //To extract the zip in a new folder
                            ZipArchive archive = ZipFile.Open(filePath, ZipArchiveMode.Update);
                            foreach (var entry in archive.Entries)
                            {
                                string path = PathCombine(DownloadPath, entry.FullName);
                                if (File.Exists(path))
                                {
                                    int j = 1;
                                    while (File.Exists(path))
                                    {
                                        string tmpFileName = string.Format("{0}({1})", Path.GetFileNameWithoutExtension(entry.Name), j++);
                                        path = Path.Combine(DownloadPath, tmpFileName + Path.GetExtension(entry.Name));
                                    }

                                }

                                //Se l'entry è inserita in una subfolder è necessario controllare l'esistenza della stessa e eventualmente crearla
                                string subfolderPath = Path.GetDirectoryName(path);
                                if (!Directory.Exists(subfolderPath))
                                    Directory.CreateDirectory(subfolderPath);

                                entry.ExtractToFile(path);
                            }
                            archive.Dispose();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            System.Windows.MessageBox.Show("You need to open the app as an administrator");
                        }
                        catch ( Exception ex)
                        when (ex is InvalidDataException ||
                              ex is ArgumentException)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);

                            if (File.Exists(filePath))
                                File.Delete(filePath);
                        }

                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        CloseConnection();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace.ToString());
                Console.WriteLine(e.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void CloseConnection() {
            try
            {
                closeConnection = true;
                var networkStream = clientSocket.GetStream();
                networkStream.Flush();
                networkStream.Close();                
            }
            catch (ObjectDisposedException) 
            {
                Console.WriteLine("The network stream was already disposed");
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("The socket was not connected");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }
            clientSocket.Close();

        }

        private string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return Path.Combine(path1, path2);
        }

        private byte[] imageToByteArray(string imgPath) {
            Image x = Image.FromFile(imgPath);
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

    }
}