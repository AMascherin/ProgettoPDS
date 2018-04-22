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
    class NetworkUserManager //TODO: sincronizzazione
    {
        private static List<NetworkUser> _userlist = new List<NetworkUser>();
        public NetworkUserManager() { }
        private readonly object _UserDatalocker = new object();
        public List<NetworkUser> userlist
        {
            get { lock (_UserDatalocker) { return _userlist; } }
        }


        public void AddUser(NetworkUser newuser)
        {
            bool checknewuser = true;
            DateTime checktime = DateTime.UtcNow;

            System.Windows.MessageBox.Show(newuser.Ipaddress);

            lock (_UserDatalocker)
            {
                for (int i = 0; i < _userlist.Count; i++)
                {
                    if (newuser.MACAddress == _userlist[i].MACAddress) //Controlla se l'utente è già stato salvato, e ne aggiorna i dati se necessario
                    {
                        _userlist[i] = newuser;
                        checknewuser = false;
                    }
                    TimeSpan diff = checktime - _userlist[i].TimeStamp;
                    if (diff.TotalSeconds > 15.0) _userlist.RemoveAt(i); //TODO: Controllare problemi con l'indice di iterazione i !!!!!!
                }
                if (checknewuser) _userlist.Add(newuser); //Aggiunge l'utente alla lista se non era presente
            }
        }

        public void SendTest(string msg)
        {
            System.Windows.MessageBox.Show("SendTest activate");
            lock (_UserDatalocker)
            {
                System.Windows.MessageBox.Show("LockObtained");
                TCPSender _sender = new TCPSender("192.168.1.103");


                _sender.SendData(msg);
            }
        }

    }




    class MainHub //TODO: gestire la responsività con gli eventi(tcp client, interfaccia grafica, cambio della flag della privacy
    {
        //   public static List<NetworkUser> userlist = new List<NetworkUser>();

        private NetworkUserManager nuc;
        public static UserConfiguration uc;
        private static UDPSender _udpsend;
        private static UDPReceiver _udprec;
        private static TCPReceiver _tcpRec;
        private static Thread _udpsendThread;
        private static Thread _udprecThread;
        private static Thread _tcprecThread;

        public MainHub()
        {
            uc = new UserConfiguration();
            _udpsend = new UDPSender();
            _udprec = new UDPReceiver();
            _tcpRec = new TCPReceiver();
            
            _udprecThread = new Thread(_udprec.StartListener);
            _udprecThread.Name = "UdpReceiverThread";
            _udpsendThread = new Thread(_udpsend.Start);
            _udpsendThread.Name = "UdpSenderThread";
            _tcprecThread = new Thread(_tcpRec.StartListener);
            _tcprecThread.Name = "TCPServerThread";
            nuc = new NetworkUserManager();
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

            if (uc.PrivacyFlag)
            {
                _udpsendThread.Start();
            }
            _udprecThread.Start();

            _tcprecThread.Start();


           /* while (true)
            {
                Thread.Sleep(5000);
                nuc.SendTest("MEssaggio di prova");

            }*/
              //           this.TCPServerStartup();

        }

        private void TCPServerStartup()
        {
            throw new NotImplementedException();

        }

        protected virtual void OnPrivacyChange(EventArgs e) { }  //Questo evento deve riattivare/disattivare l'UDP Sender
        protected virtual void OnSendRequest() { } //EventArgs contiene la lista di utenti a cui inviare i dati?  -->Integrare in SchermataInvio.cs
        protected virtual void CancelTransfer() { } //Andrà identificata la singola connessione
        protected virtual void OnReceiveRequest() { } //Richiesta di ricezione di fail da parte di altri utente





    }
}
