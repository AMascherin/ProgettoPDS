using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PDSProject
{
    class UserConfiguration
    {

        private static bool _PrivacyFlag; //TRUE: Pubblic, FALSE: Private
        private static string _Username;
        private static string _ImgPath;
        private static bool mustsaveflag;
        private static string _multicastaddress;
        private readonly object _UserDatalocker = new object(); 

        /*TRUE=ci sono nuovi dati da salvare. -->Bisogna chiamare un dump
         FALSE=i dati contenuti in userconfiguration corrispondono a quelli contenuti nel file di configurazione su disco
         */
        public string multicastaddress { //Sola lettura. La variabile viene settata solo dalla LoadConfiguration (TODO: Gestire parsing JSON)
            get {
                return _multicastaddress;
            }
        }

                
        public bool PrivacyFlag {
            get {
                lock (_UserDatalocker)
                {
                    return _PrivacyFlag;
                }
            }
            set {
                lock (_UserDatalocker)
                {
                    _PrivacyFlag = value;
                    mustsaveflag = true;
                    DumpConfiguration("Path"); //Gestione di questo parametro. Dump per ogni paramentro o soluzione alternativa? 
                }
            }
        } 
        public string Username  { get { return _Username;    }   set { _Username = value;    } }
        public string ImgPath   { get { return _ImgPath;     }   set { _ImgPath = value;     } }

        public UserConfiguration() { }

        [JsonConstructor]
        public UserConfiguration(bool flag, string user, string img, string multicastaddress) {
            _PrivacyFlag = flag;
            _Username = user;
            _ImgPath = img;
            _multicastaddress = multicastaddress;
            mustsaveflag = false;
        }

        private UserConfiguration(bool Flag, string user, string img)
        {
            _PrivacyFlag = Flag;
            _Username = user;
            _ImgPath = img;
 //           mustsaveflag = false;
        }

        //Eccezioni da gestire per il path
        /*
         ArgumentException,DirectoryNotFoundException,DirectoryNotFoundException,UnauthorizedAccessException,FileNotFoundException
             
        
        */

        public void DumpConfiguration(string path) {  //TODO: Gestione eccezioni  -->possibili eccezioni sono date da path errati o permessi mancanti
            string json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(@"path", json); 
        }

        public void LoadConfiguration (string path) //TODO: Gestione eccezioni
        {
                String JSONstring = File.ReadAllText(@path); 
                UserConfiguration user = JsonConvert.DeserializeObject<UserConfiguration>(JSONstring);
        }



        //private static void SaveConfiguration()
        //{
            
        //}

        //public void StartConfigurationRoutine()
        //{
        //    Thread savingroutine = new Thread(SaveConfiguration); //TODO: Trovare soluzione alternativa
        //    savingroutine.Name = "savingroutine";
        //    savingroutine.Priority = ThreadPriority.Lowest;
        //}
    }
}