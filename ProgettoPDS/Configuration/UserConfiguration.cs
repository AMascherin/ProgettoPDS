﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProgettoPDS
{
    class UserConfiguration
    {

        private static bool _PrivacyFlag; //TRUE: Pubblic, FALSE: Private
        private static string _Username;
        private static string _ImgPath;
        private static string _multicastaddress;
        private readonly object _UserDatalocker = new object();

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
            get { lock (_UserDatalocker) {  return _PrivacyFlag;  } }
            set { lock (_UserDatalocker) {  _PrivacyFlag = value; } }
        }
        public string Username
        {
            get { lock (_UserDatalocker) {  return _Username;    } }
            set { lock (_UserDatalocker) {  _Username = value;   } }
        }

        public string ImgPath
        {
            get { lock (_UserDatalocker) {  return _ImgPath;     } }
            set { lock (_UserDatalocker) {  _ImgPath = value;    } }
        }
        
        public UserConfiguration() //Costruttore
        {
            if (ImgPath == null)
            {
                SetDefaultPath(); //Se non è definita un'immagine, viene utilizzata quella di deafult
            }

        }

        [JsonConstructor]
        public UserConfiguration(bool flag, string user, string img, string multicastaddress)
        {
            lock (_UserDatalocker)
            {
                _PrivacyFlag = flag;
                _Username = user;
                _ImgPath = img;
                _multicastaddress = multicastaddress;
            }
        }


        public void DumpConfiguration() //Salvataggio dei dati utenti nel file di configurazione in formato JSON
        {

            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string fullpath = currentfolder + @"\config.json";
            if (ImgPath == null) {
                ImgPath = currentfolder + @"\Media\images.png";
             }
            if (multicastaddress == null) {
                lock (_UserDatalocker) {
                    _multicastaddress = "239.10.10.10";
                }               
            }
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

        private string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }

        private string getTimeStamp() {
            DateTime utcDate = DateTime.UtcNow;
            return utcDate.ToString();
        }

        public string GetJSONConfiguration() {  
            //TODO: Definire un campo per il MAC address e un campo per specificare se l'immagine utente è nuova

            JTokenWriter writer = new JTokenWriter();
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(Username);
            writer.WritePropertyName("MACAddress"); 
            writer.WriteValue(GetMACAddress()); //TODO:Testare e implementare ricezione
            writer.WritePropertyName("DefaultImage");
            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);            
            bool defimg = (ImgPath == (currentfolder + @"\Media\images.png")) ? true : false;
            writer.WriteValue(defimg);
            writer.WritePropertyName("Timestamp");
            writer.WriteValue(getTimeStamp());
            JObject o = (JObject)writer.Token;
            return o.ToString();
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
                throw ex; 
            }
        }

        public void SetDefaultPath() {
            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            ImgPath = currentfolder + @"\Media\images.png";
        }
    }
}