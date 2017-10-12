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
        public static List<NetworkUser> userlist;
        public static UserConfiguration uc;
        private static UDPSender _udpsend;
        private static Thread _udpsendThread;
        public MainHub()
        {
            userlist = new List<NetworkUser>();
            uc = new UserConfiguration();
            _udpsend = new UDPSender("224.0.0.25"); //TODO: Gestione indirizzo multicast
            _udpsendThread = new Thread(_udpsend.Start);
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
                    //TODO: Questo metodo deve essere eseguito se e solo se alla chiusura della mainwindow non sia stato generato il file di configurazione
                    string msg = "Non sono stati inseriti i dati richiesti"; 
                    System.Windows.MessageBox.Show(msg);
                    return;
                }
                if (uc.PrivacyFlag) {
                    _udpsendThread.Start();
                }

                //TODO: creare un thread per UDPlistener
                this.TCPServerStartup();
            }
        }

        private void TCPServerStartup()
        {
            throw new NotImplementedException();

        }

    }
}
