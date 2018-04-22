using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace ProgettoPDS
{
    //TODO: Aumentare il TTL dei pacchetti UDP
    class UDPSender
    {
        private static UdpClient udpclient;
        private static IPAddress groupAddress;
        private IPEndPoint Clientdest;
        private UserConfiguration user;

        public UDPSender() 

        {
            user = new UserConfiguration();
            udpclient = new UdpClient();
            groupAddress=IPAddress.Parse("255.255.255.255");  //IPv4 Broadcast
            Clientdest = new IPEndPoint(groupAddress, 13370); //Hard-Coded Port Number. TODO: Check confirmation user authorization
        }

        public void Start()
        {
            while (true)
            {                
                if (user.PrivacyFlag)
                {

                    try
                    {
                        string data = user.GetJSONConfiguration();
                        Byte[] data_b = Encoding.Unicode.GetBytes(data);
                        udpclient.Send(data_b, data_b.Length, Clientdest);
                    }
                    catch (ArgumentNullException ex)
                    {
                        break;

                    }
                    catch (EncoderFallbackException ex)
                    {
                        break;

                    }
                }
                else break;
                Thread.Sleep(10000);

            }
        }

        public void SingleStart() {

            string data = user.GetJSONConfiguration();
            try
            {
                udpclient.Send(Encoding.Unicode.GetBytes(data), data.Length, Clientdest);
            }
            catch(ArgumentNullException ex)
            {
                Application.Exit();

            }
            catch (InvalidOperationException ex)
            {

                string message = "Invalid Operation. Please try again";
                string caption = "Invalid operation";

                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption);


            }
            catch (SocketException ex)
            {

                string message = "Socket error. Please check network settings";
                string caption = "Connection Error";
                
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption);
                


            }

        }
    }
}
