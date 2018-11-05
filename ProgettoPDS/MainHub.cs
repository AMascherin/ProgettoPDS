using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ProgettoPDS
{

    class MainHub 
    {
        public static UserConfiguration uc;
        private static UDPSenderManager _udpsend;
        private static UDPReceiver _udprec;
        private TcpReceiver _tcpReceiver;
        private NetworkUserListCleaner listCleaner;

        private static Thread _udpsenderManagerThread;
        private static Thread _udprecThread;
        private Thread _tcprecThread;
        private Thread _tcplocalhostthread;
        private Thread _listCleanerThread;

        public MainHub()
        {
            uc = new UserConfiguration();
            UserConfiguration.PrivacyFlagChanged += uc_PrivacyChanged;
            
            _udpsend = new UDPSenderManager();
            _udprec = new UDPReceiver();
            _tcpReceiver = new TcpReceiver();
            var tcplocalhost = new TCP_LocalHostReceiver();

            _udprecThread = new Thread(_udprec.StartListener);
            _udprecThread.Name = "UdpReceiverThread";

            _udpsenderManagerThread = new Thread(_udpsend.Start);
            _udpsenderManagerThread.Name = "UdpSenderManagerThread";

            _tcprecThread = new Thread(_tcpReceiver.StartListener);
            _tcprecThread.Name = "TCPServerThread";
            _tcplocalhostthread = new Thread(tcplocalhost.StartListener);
            _tcplocalhostthread.Name = "TCPLocalHost";

        }

        [STAThread]
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
                //Non esistono i dati inseriti dall'utente
                OptionWindow mw1 = new OptionWindow();
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
            
            _udpsenderManagerThread.Start();
            _udprecThread.Start();
            _tcprecThread.Start();
            _tcplocalhostthread.Start();

            listCleaner = new NetworkUserListCleaner();
            _listCleanerThread = new Thread(listCleaner.StartCleaner);
            _listCleanerThread.Start();
                                    
        }

        public void CleanUp()
        {
            listCleaner.TerminateThread = true;
            _listCleanerThread.Join();

        }

        static void uc_PrivacyChanged(object sender, PrivacyChangedEventArgs e) {
            if (e.flag)
            {
                _udpsend.Start();
            }
            else
            {
                _udpsend.Stop();
            }
            Console.WriteLine("The privacy was changed with value: "+e.flag);
        }
    }
}

