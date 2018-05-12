using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProgettoPDS
{
    class TCPSender
    {
        private int PORT_NO = 13370;

        private TcpClient client;

        public TCPSender(string ip)
        {
            client = new TcpClient(ip, PORT_NO);
            System.Windows.MessageBox.Show("TCP Sender created");
        }

        private void SendData(string pathToObj, NetworkStream nwStream)
        {
            try
            {
                //---create a NetworkStream---
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

            byte[] inStream = new byte[chunkSize]; //TODO: Check overflow
            nwStream.Read(inStream, 0, (int)client.ReceiveBufferSize);
            string returndata = System.Text.Encoding.UTF8.GetString(inStream);
            Console.WriteLine("Data from Server : " + returndata);
            nwStream.FlushAsync();

            if (returndata.Equals("200 OK"))
            {
                foreach (String file in filesToSend)
                {
                    SendData(file, nwStream);
                }

                /* nwStream.Read(inStream, 0, (int)client.ReceiveBufferSize);
                 string response = System.Text.Encoding.UTF8.GetString(inStream);
                 Console.WriteLine("Data from Server : " + response);*/
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

        /*Remove in future update
         * public void SendMessage(Object obj)
        {

            try
            {
                //---create a NetworkStream---
                NetworkStream nwStream = client.GetStream();

                //Convert message string into byte data
                byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes((string)obj);

                //Send data into stream
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                nwStream.Close();
                // CloseConnection();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }*/
    }
}

