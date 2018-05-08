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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ProgettoPDS
{
    class TCPSender
    {


        private int PORT_NO = 13370;
       // private static IPAddress targetAddress;
       // private IPEndPoint targetEndPoint;
      //  private string SERVER_IP = "127.0.0.1";
        private static TcpClient client;

        public TCPSender(string ip)
        {
            //   targetAddress = IPAddress.Parse(ip);
            //   targetEndPoint = new IPEndPoint(targetAddress, PORT_NO);
            //   client = new TcpClient(targetEndPoint);
            client = new TcpClient(ip, PORT_NO);
            System.Windows.MessageBox.Show("TCP Sender create");
        }

        public void SendData (string pathToObj)
        {
            try {
                //---create a NetworkStream---
                NetworkStream nwStream = client.GetStream();

                byte[] bytesToSend = File.ReadAllBytes(pathToObj);

                //Send data into stream
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                //nwStream.Close();
                //CloseConnection();
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


        public void SendMessage(Object obj)
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

        }

        public void CloseConnection()
        {
            client.Close();
        }

        /*public void SendData(Object data)
         {

             Thread t = new Thread(new ParameterizedThreadStart(SendMessage));
             t.Start(data);
             //t.detach();

          }*/

        public void HandleFileSend(List<String> filesToSend)
        {

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


            /*
 {
	'files': [
		'file1':{
			'name': ...
			'size': ...
			'extension': ...			
		}
		
		'file2':{		
		
		}	
	}
}       
             */

            String jsonString = "pippo";           


            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes((string)jsonString.ToString()); //TODO: serve davvero?
            nwStream.Write(bytesToSend, 0, bytesToSend.Length); //Send to the server the file information data
            nwStream.Flush();

            byte[] inStream = new byte[10025]; //TODO: Check 
            nwStream.Read(inStream, 0, (int)client.ReceiveBufferSize);
            string returndata = System.Text.Encoding.UTF8.GetString(inStream);
            Console.WriteLine("Data from Server : " + returndata);
            nwStream.FlushAsync();

            if (returndata.Equals("200 OK"))
            {
                foreach (String file in filesToSend) {
                    SendData(file);
                }
                
                nwStream.Read(inStream, 0, (int)client.ReceiveBufferSize);
                string response = System.Text.Encoding.UTF8.GetString(inStream);
                Console.WriteLine("Data from Server : " + response);
                nwStream.Close();
                CloseConnection();


            }
            else {
                System.Windows.MessageBox.Show("Request rejected by the server, closing connection");
                nwStream.Close();
                CloseConnection();               
            }
            
            throw new NotImplementedException();
        }
    }
}

