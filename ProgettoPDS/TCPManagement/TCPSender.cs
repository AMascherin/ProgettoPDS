using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ProgettoPDS
{
    class TCPSender
    {
        private int PORT_NO = 13370;

        private TcpClient client;

        public TCPSender(string ip)
        {
            IPAddress ipAd = IPAddress.Parse(ip);

            client = new TcpClient(ipAd.ToString(), PORT_NO);

            System.Windows.MessageBox.Show("TCP Sender created");
        }

        private void SendData(string pathToObj, NetworkStream nwStream)
        {
            try
            {
                //NetworkStream nwStream = client.GetStream();

                byte[] bytesToSend = File.ReadAllBytes(pathToObj);

                //Send data into stream
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException: {0}", e);
            }
        }

        private void CloseConnection()
        {
            client.Close();
        }

        public void handleFileSend(List<String> filesToSend)
        {
            int chunkSize = 2048;
            List<FileInfo> filesinfo = new List<FileInfo>();

            //---create a NetworkStream---
            NetworkStream nwStream = client.GetStream();

            foreach (String file in filesToSend)
            {
                filesinfo.Add(new FileInfo(file));

            }

            JObject jsonfile = new JObject();

            for (int i = 0; i < filesinfo.Count; i++)
            {
                jsonfile.Add(new JProperty("File" + i,
                new JObject(
                    new JProperty("nome", filesinfo[i].Name),
                    new JProperty("estensione", filesinfo[i].Extension),
                    new JProperty("dimensione", filesinfo[i].Length))));
            }


            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes((string)jsonfile.ToString());
            nwStream.Write(bytesToSend, 0, bytesToSend.Length); //Send to the server the file information data
            nwStream.Flush();
            byte[] inStream = new byte[chunkSize];
               
            int clientMessage = nwStream.Read(inStream, 0, inStream.Length); //Leggo il messaggio dell'utente
            String returndata = System.Text.Encoding.UTF8.GetString(inStream).TrimEnd('\0');
            
            System.Windows.MessageBox.Show("Data from Server : " + returndata);
           // nwStream.FlushAsync();

            if (returndata.Equals("200 OK"))
            {
                //https://www.codeguru.com/csharp/.net/zip-and-unzip-files-programmatically-in-c.htm
                string zipPath = "Test";
                using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (String file in filesToSend)
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
                        //SendData(file, nwStream);
                        // byte[] bytesArrayToSend = File.ReadAllBytes(file); //MAX 2 GB

                        //Send data into stream
                        //nwStream.Write(bytesArrayToSend, 0, bytesArrayToSend.Length);

                        //https://stackoverflow.com/questions/21259703/how-to-receive-large-file-over-networkstream-c
                        /*  var fileIO = File.OpenRead(file) ;

                          var bytesArrayToSend = new byte[1024 * 8];
                          int count;
                          while ((count = fileIO.Read(bytesArrayToSend, 0, bytesArrayToSend.Length)) > 0)
                              nwStream.Write(bytesArrayToSend, 0, count);*/

                        //TODO: Send message to signal end of transmission (dati: file inviato, numero di file mancanti)
                    }
                    using (var fileIO = File.OpenRead(zipPath))
                    {
                        var bytesArrayToSend = new byte[1024 * 8];
                        int count;
                        while ((count = fileIO.Read(bytesArrayToSend, 0, bytesArrayToSend.Length)) > 0)
                            nwStream.Write(bytesArrayToSend, 0, count);
                    }

                } //zip.Dispose();




                nwStream.Close();
                CloseConnection();


            }
            else
            {
                System.Windows.MessageBox.Show("Request rejected by the server, closing connection");
                nwStream.Close();
                CloseConnection();
            }
        }

   }
}

