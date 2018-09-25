using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Diagnostics;

namespace ProgettoPDS
{
    class TCPSender
    {
       // private int PORT_NO = 13370;

        private TcpClient client;

        UserConfiguration uc;

        public TCPSender(string ip)
        {
            IPAddress ipAd = IPAddress.Parse(ip);

            client = new TcpClient(ipAd.ToString(), uc.GetTCPPort());

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

        public void handleFileSend(List<String> filesPathToSend)
        {
            int chunkSize = 2048;

            //---create a NetworkStream---
            NetworkStream nwStream = client.GetStream();

            JObject jsonfile = new JObject();
            int i = 0;
            foreach (String file in filesPathToSend)
            {                
                if (File.Exists(file))
                {
                   FileInfo fileInfo = new FileInfo(file);
                   jsonfile.Add(
                        new JProperty("File" + i,
                            new JObject(
                                new JProperty("nome", fileInfo.Name),
                                new JProperty("estensione", fileInfo.Extension),
                                new JProperty("dimensione", fileInfo.Length)
                            )
                        )
                    );
                    i++;
                }
                if (Directory.Exists(file)) {
                    DirectoryInfo d = new DirectoryInfo(file);
                    //Aggiungere al JSon una qualche informazione relativa alla cartella (nome, peso, contenuto)
                    jsonfile.Add(
                        new JProperty("File" + i,
                            new JObject(
                                new JProperty("nome", Path.GetDirectoryName(file)),
                                new JProperty("estensione", "Folder"),
                                new JProperty("dimensione", GetDirectorySize(file))
                            )
                        )
                    );
                    i++;
                }
            }


            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes((string)jsonfile.ToString());
            nwStream.Write(bytesToSend, 0, bytesToSend.Length); //Send to the server the file information data
            nwStream.Flush();
            byte[] inStream = new byte[chunkSize];
               
            int clientMessage = nwStream.Read(inStream, 0, inStream.Length); //Leggo il messaggio dell'utente
            String returndata = System.Text.Encoding.UTF8.GetString(inStream).TrimEnd('\0');
            
            System.Windows.MessageBox.Show("Data from Server : " + returndata);

            if (returndata.Equals("200 OK"))
            {
                //https://www.codeguru.com/csharp/.net/zip-and-unzip-files-programmatically-in-c.htm

                string zipPath = "Test.zip"; //TODO

                using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (String inputPath in filesPathToSend)
                    {
                        if (File.Exists(inputPath))
                            zip.CreateEntryFromFile(inputPath, Path.GetFileName(inputPath), CompressionLevel.Fastest);
                        else if (Directory.Exists(inputPath)) {
                            var directoryInfo = Directory.GetParent(inputPath);

                            foreach (var filePath in System.IO.Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories))
                            {
                                var relativePath = filePath.Replace(directoryInfo.Parent.FullName, string.Empty);
                                using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                using (Stream fileStreamInZip = zip.CreateEntry(relativePath).Open())
                                    fileStream.CopyTo(fileStreamInZip);
                            }

                        }

                        //https://stackoverflow.com/questions/21259703/how-to-receive-large-file-over-networkstream-c
                    }
                    zip.Dispose();
                    using (var fileIO = File.OpenRead(zipPath))
                    {
                        var bytesArrayToSend = new byte[1024 * 8];
                        int count;
                        while ((count = fileIO.Read(bytesArrayToSend, 0, bytesArrayToSend.Length)) > 0)
                            nwStream.Write(bytesArrayToSend, 0, count);
                    }
                } 
                
                nwStream.Close();
                CloseConnection();
                if(File.Exists(zipPath))
                    File.Delete(zipPath);

            }
            else
            {
                System.Windows.MessageBox.Show("Request rejected by the server, closing connection");
                nwStream.Close();
                CloseConnection();
            }
        }

        public void SendImageRequest(string filename) {
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes("Send Image");
            nwStream.Write(bytesToSend, 0, bytesToSend.Length); 
            nwStream.Flush();

            lock (this)
            {
                try
                {                    
                    Stream fileStream = File.OpenWrite(filename);
                    byte[] clientData = new byte[2048];
                    int bytesread = nwStream.Read(clientData, 0, clientData.Length); 
                    while (bytesread > 0)
                    {
                        fileStream.Write(clientData, 0, bytesread);
                        bytesread = nwStream.Read(clientData, 0, clientData.Length);
                    }
                    fileStream.Close();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                    System.Windows.MessageBox.Show(e.StackTrace);
                }
            }
            nwStream.Close();
            client.Close();
        }

        static long GetDirectorySize(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, " *.*");

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }
    }
}

