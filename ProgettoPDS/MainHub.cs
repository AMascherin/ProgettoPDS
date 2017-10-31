using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ProgettoPDS
{

    class MainHub //TODO: gestire la responsività con gli eventi(tcp client, interfaccia grafica, cambio della flag della privacy
    {
        public static List<NetworkUser> userlist = new List<NetworkUser>();
        public static UserConfiguration uc;
        private static UDPSender _udpsend;
        private static UDPReceiver _udprec;
        private static Thread _udpsendThread;


        public MainHub()
        {
            uc = new UserConfiguration();
            _udpsend = new UDPSender();
            _udprec = new UDPReceiver();
            _udpsendThread = new Thread(_udpsend.Start);
            _udpsendThread.Name = "UdpSenderThread";
        }

        public void Initialize()
        {
            /*Caricamento della configurazione utente, ed eventuale gestione della mancanza del file di configurazione
             Avvio del server TCP
             Avvio del listener UDP
             Avvio del sender UDP, se la flag lo prevede          
             */
            try
            {
                uc.LoadConfiguration();
            }
            catch (IOException)
            {
                OptionWindow mw1 = new OptionWindow(true);
                mw1.ShowDialog();
                string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string fullpath = folder + @"\config.json";
                if (!File.Exists(fullpath))
                {
                    string msg = "Non sono stati inseriti i dati richiesti";
                    System.Windows.MessageBox.Show(msg);
                    return;
                }
            }

            if (uc.PrivacyFlag) {
                  _udpsendThread.Start();
            }
            _udprec.StartListener();

   //             this.TCPServerStartup();
    
        }

        private void TCPServerStartup()
        {
            throw new NotImplementedException();

        }

        protected virtual void OnPrivacyChange(EventArgs e)  { }  //Questo evento deve riattivare/disattivare l'UDP Sender
        protected virtual void OnSendRequest() { } //EventArgs contiene la lista di utenti a cui inviare i dati?  -->Integrare in SchermataInvio.cs
        protected virtual void CancelTransfer() { } //Andrà identificata la singola connessione
        protected virtual void OnReceiveRequest() { } //Richiesta di ricezione di fail da parte di altri utente





    }
}
