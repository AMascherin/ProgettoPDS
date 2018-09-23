using System;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileSenderApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //TODO1: Lanciare Progetto PDS. Se non esistente segnalare all'utente che c'è un problema con l'installazione
            //TODO2: Leggere il file di configurazione. Se non presente chiudere l'applicazione
            

            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream,ProtocolType.Tcp);
                client.Connect(remoteEP);
                byte[] msg = Encoding.ASCII.GetBytes(e.Args[0]);
                // Send the data through the socket.  
                int bytesSent = client.Send(msg);

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close(5);
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
            }


           var _invio = new InvioFile();
            _invio.Show();

        }

    }
}
