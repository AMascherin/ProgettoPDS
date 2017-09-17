using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    class UserConfiguration
    {

        private static bool _PrivacyFlag; //TRUE: Pubblic, FALSE: Private
        private static string _Username;
        private static string _ImgPath;
        private static bool mustsaveflag;
   //     public readonly object _UserDatalocker = new object(); 
   //TODO: Risolvere il problema della gestione del locker

        /*TRUE=ci sono nuovi dati da salvare. Viene svegliato il thread a bassa priorità per salvare la configurazione. Se in fase di terminazione del programma ci sono ancora dati
         * da salvare, si forza un salvataggio
         FALSE=i dati contenuti in userconfiguration corrispondono a quelli contenuti nel file di configurazione su disco
         */
            
        public bool PrivacyFlag { get { return _PrivacyFlag; }   set { _PrivacyFlag = value; } } 
        public string Username  { get { return _Username;    }   set { _Username = value;    } }
        public string ImgPath   { get { return _ImgPath;     }   set { _ImgPath = value;     } }

        public UserConfiguration() { }

        public UserConfiguration(bool Flag, string user, string img)
        {
            _PrivacyFlag = Flag;
            _Username = user;
            _ImgPath = img;
            mustsaveflag = false;
        }

        public void DumpConfiguration() {  //TODO: Gestione eccezioni e path come parametro -->possibili eccezioni sono date da path errati o permessi mancanti
            string json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(@"C:\Users\Alessandro\Desktop\config.json", json); 
        }

        public void LoadConfiguration() //TODO: Gestione eccezioni e path come parametro
        {
                String JSONstring = File.ReadAllText(@"C:\Users\Alessandro\Desktop\config.json"); 
                UserConfiguration user = JsonConvert.DeserializeObject<UserConfiguration>(JSONstring);
        }



        private static void SaveConfiguration()
        {
            
        }



        public void StartConfigurationRoutine()
        {

            Thread savingroutine = new Thread(SaveConfiguration); //TODO: Trovare soluzione alternativa

            savingroutine.Name = "savingroutine";

            savingroutine.Priority = ThreadPriority.Lowest;

        }
    }
}