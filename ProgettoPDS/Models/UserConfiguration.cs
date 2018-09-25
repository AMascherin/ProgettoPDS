using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProgettoPDS
{
    class UserConfiguration
    {

        private static bool _PrivacyFlag; //TRUE: Pubblic, FALSE: Private
        private static bool _AutomaticDownloadAcceptance;
        private static string _Username;
        private static string _ProfileImagePath;
        private static string _ImgLastModified;
        private static bool _DefaultDownloadPathFlag; //If true, the user has provided a default download path for all the files
        private static string _DefaultDownloadPathString;
       

        private readonly object _UserDatalocker = new object();

        public String GetMulticastUDPAddress () {
            return "224.5.5.5";
        }

        public int GetUDPPort() {
            return 13300;
        }

        public int GetTCPPort() {
            return 13301;
        }

        public bool AutomaticDownloadAcceptance
        {
            get { lock (_UserDatalocker) { return _AutomaticDownloadAcceptance; } }
            set { lock (_UserDatalocker) { _AutomaticDownloadAcceptance = value; } }
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
            get { lock (_UserDatalocker) {  return _ProfileImagePath;     } }
            set {
                lock (_UserDatalocker) {
                    _ProfileImagePath = value;
                    ImgLastModified = getTimeStamp();
                }
            }
        }

        public bool DefaultDownloadPath
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _DefaultDownloadPathFlag;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    _DefaultDownloadPathFlag = value;
                }
            }
        }

        public string DefaultDownloadPathString
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _DefaultDownloadPathString;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    _DefaultDownloadPathString = value;
                }
            }
        }

        public string ImgLastModified
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _ImgLastModified;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    _ImgLastModified = value;
                }
            }
        }

        public UserConfiguration() //Costruttore
        {
            if (ImgPath == null)
            {
                SetDefaultPath(); //Se non è definita un'immagine, viene utilizzata quella di deafult
            }
            
        }

        [JsonConstructor]
        public UserConfiguration(bool flag, string user, string img)
        {
            lock (_UserDatalocker)
            {
                _PrivacyFlag = flag;
                _Username = user;
                _ProfileImagePath = img;
            }
        }


        public void DumpConfiguration() //Salvataggio dei dati utenti nel file di configurazione in formato JSON
        {

            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string fullpath = currentfolder + @"\config.json";
            if (ImgPath == null) {
                ImgPath = currentfolder + @"\Media\images.png";
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

        public string GetMACAddress()
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
            writer.WriteValue(GetMACAddress()); 
            writer.WritePropertyName("DefaultImage");
            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);            
            bool defimg = (ImgPath == (currentfolder + @"\Media\images.png")) ? true : false;
            writer.WriteValue(defimg);
            writer.WritePropertyName("Image change timestamp");
            writer.WriteValue(ImgLastModified);
            writer.WritePropertyName("Packet Timestamp");
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
            ImgLastModified = getTimeStamp();
        }
            
    }
}