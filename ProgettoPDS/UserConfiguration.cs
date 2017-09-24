using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProgettoPDS
{
    class UserConfiguration
    {

        private static bool _PrivacyFlag; //TRUE: Pubblic, FALSE: Private
        private static string _Username;
        private static string _ImgPath;
        //       private static bool initializedFlag;
        private static string _multicastaddress;
        private readonly object _UserDatalocker = new object();

        /*TRUE=ci sono nuovi dati da salvare. -->Bisogna chiamare un dump
         FALSE=i dati contenuti in userconfiguration corrispondono a quelli contenuti nel file di configurazione su disco
         */
        public string multicastaddress
        { //Sola lettura. La variabile viene settata solo dalla LoadConfiguration (TODO: Gestire parsing JSON)
            get
            {
                lock (_UserDatalocker)
                {
                    return _multicastaddress;
                }
            }
        }
        public bool PrivacyFlag
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _PrivacyFlag;
                }
            }
            set
            {
                lock (_UserDatalocker)
                {
                    _PrivacyFlag = value;
                }
            }
        }
        public string Username
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _Username;
                }
            }
            set
            {
                lock (_UserDatalocker)
                {
                    _Username = value;
                }
            }
        }
        public string ImgPath
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _ImgPath;
                }
            }
            set
            {
                lock (_UserDatalocker)
                {
                    _ImgPath = value;
                }
            }
        }

        public UserConfiguration() { }

        [JsonConstructor]
        public UserConfiguration(bool flag, string user, string img, string multicastaddress)
        {
            lock (_UserDatalocker)
            {
                _PrivacyFlag = flag;
                _Username = user;
                _ImgPath = img;
                _multicastaddress = multicastaddress;
                //               initializedFlag = true;
            }
        }


        public void DumpConfiguration() 
        {

            string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string fullpath = folder + @"\config.json";
            string json = JsonConvert.SerializeObject(this);
            try
            {
                System.IO.File.WriteAllText(fullpath, json);
            }
            catch (UnauthorizedAccessException ec) {
                System.Windows.MessageBox.Show(ec.ToString());
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());

            }
        }

        public string GetJSONConfiguration() {
            return JsonConvert.SerializeObject(this);     //TODO: Non inviare il path e il multicast address. Controllare la dimensione del risultato
        }

        public int LoadConfiguration()
        {
            try
            {
                string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string fullpath = folder + @"\config.json";
                String JSONstring = File.ReadAllText(@fullpath);
                UserConfiguration user = JsonConvert.DeserializeObject<UserConfiguration>(JSONstring);
                return 1;
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                throw ex; //TODO: Il chiamante deve chiedere all'utente di reinserire i dati
            }
        }
    }
}