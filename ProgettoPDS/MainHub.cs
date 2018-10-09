using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProgettoPDS
{
    static class NetworkUserManager //TODO: sincronizzazione
    {
        private static List<NetworkUser> _userlist = new List<NetworkUser>();
        private static readonly object _UserDatalocker = new object();
        public static List<NetworkUser> userlist
        {
            get { lock (_UserDatalocker) { return _userlist; } }
        }


        public static void AddUser(NetworkUser newuser)
        {
            bool checknewuser = true;
            DateTime checktime = DateTime.UtcNow;

            lock (_UserDatalocker)
            {
                for (int i = 0; i < _userlist.Count; i++)
                {
                    if (newuser.MACAddress.Equals(_userlist[i].MACAddress)) //Controlla se l'utente è già stato salvato, e ne aggiorna i dati se necessario
                    {                        
                        if (!newuser.DefaultImage) {
                            int result = DateTime.Compare(newuser.ImageTimeStamp, _userlist[i].ImageTimeStamp);
                            string filename;
                            if(result < 0) {
                                // L'immagine non è di default ed è stata cambiata 
                                TCPSender sender = new TCPSender(newuser);
                                string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                                filename = currentfolder + @"\Media\" + newuser.MACAddress.ToString() + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".png";
                                sender.SendImageRequest(filename);
                                Console.WriteLine("Image changed");
                            }
                            else {
                                filename = _userlist[i].Imagepath;
                            }
                            
                            _userlist[i] = newuser;
                            _userlist[i].Imagepath = filename;
                        }

                        else
                        {
                            _userlist[i] = newuser;
                        }                       
                       
                        checknewuser = false;
                    }
                    //TimeSpan diff = checktime - _userlist[i].TimeStamp;
                    //if (diff.TotalSeconds > 15.0) _userlist.RemoveAt(i); //TODO: Controllare problemi con l'indice di iterazione i !!!!!!
                }
                if (checknewuser)
                {
                    _userlist.Add(newuser); //Aggiunge l'utente alla lista se non era presente
                    if (!newuser.DefaultImage) {
                        TCPSender sender = new TCPSender(newuser);
                        string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        string filename = currentfolder + @"\Media\" + newuser.MACAddress.ToString() + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".png";
                        sender.SendImageRequest(filename);
                        _userlist[_userlist.Count - 1].Imagepath = filename;
                    }
                }
                
            }
        }        


    }


    class MainHub //TODO: gestire la responsività con gli eventi(tcp client, interfaccia grafica, cambio della flag della privacy
    {
        public static UserConfiguration uc;
        private static UDPSender _udpsend;
        private static UDPReceiver _udprec;
        private TcpReceiver _tcpReceiver;
        private static Thread _udpsendThread;
        private static Thread _udprecThread;
        private Thread _tcprecThread;
        private Thread _tcplocalhostthread;

        public MainHub()
        {
            uc = new UserConfiguration();
            _udpsend = new UDPSender();
            _udprec = new UDPReceiver();
            _tcpReceiver = new TcpReceiver();
            var tcplocalhost = new TCP_LocalHostReceiver();

            _udprecThread = new Thread(_udprec.StartListener);
            _udprecThread.Name = "UdpReceiverThread";
            _udpsendThread = new Thread(_udpsend.Start); 
            _udpsendThread.Name = "UdpSenderThread";
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
            try //change to if(File(exist)
            {
                uc.LoadConfiguration();
            }
            catch (IOException)
            {
                //Non esistono i dati inseriti dall'utente
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
            _tcplocalhostthread.Start();

           // NetworkUser userProva = new NetworkUser();
           //userProva.DefaultImage = true;
           //userProva.Username = "ProvaUsername";
           //NetworkUserManager.AddUser(userProva);
                        
        }

        protected virtual void OnPrivacyChange(EventArgs e) { }  //Questo evento deve riattivare/disattivare l'UDP Sender
        protected virtual void OnSendRequest() { } //EventArgs contiene la lista di utenti a cui inviare i dati?  -->Integrare in SchermataInvio.cs
        protected virtual void CancelTransfer() { } //Andrà identificata la singola connessione

        
       

    }
}

