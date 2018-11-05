using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ProgettoPDS
{

    static class NetworkUserManager
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
            

            lock (_UserDatalocker)
            {
                for (int i = 0; i < _userlist.Count; i++)
                {
                    if (newuser.MACAddress.Equals(_userlist[i].MACAddress)) //Controlla se l'utente è già stato salvato, e ne aggiorna i dati se necessario
                    {
                        if (!newuser.DefaultImage)
                        {
                            int result = DateTime.Compare(newuser.ImageTimeStamp, _userlist[i].ImageTimeStamp);
                            string filename;
                            if (result < 0)
                            {
                                // L'immagine non è di default ed è stata cambiata 
                                TCPSender sender = new TCPSender(newuser);
                                string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                                filename = currentfolder + @"\Media\Icons\" + newuser.MACAddress.ToString() + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".png";
                                sender.SendImageRequest(filename);
                                Console.WriteLine("Image changed");
                            }
                            else
                            {
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
                    
                }
                if (checknewuser)
                {
                    _userlist.Add(newuser); //Aggiunge l'utente alla lista se non era presente
                    if (!newuser.DefaultImage)
                    {
                        TCPSender sender = new TCPSender(newuser);
                        string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        string filename = currentfolder + @"\Media\Icons\" + newuser.MACAddress.ToString() + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".png";
                        sender.SendImageRequest(filename);
                        _userlist[_userlist.Count - 1].Imagepath = filename;
                    }
                }

            }
        }
    }

    public class NetworkUserListCleaner
    {
        public volatile bool TerminateThread = false;

        public void StartCleaner()
        {

            while (!TerminateThread)
            {
                DateTime checktime = DateTime.UtcNow;
                for (int i = 0; i< NetworkUserManager.userlist.Count; i++)
                {
                    TimeSpan diff = checktime - NetworkUserManager.userlist[i].TimeStamp;
                    if (diff.TotalSeconds > 15.0) NetworkUserManager.userlist.RemoveAt(i); 
                }
                Thread.Sleep(15000);
            }
        }

    }
}
