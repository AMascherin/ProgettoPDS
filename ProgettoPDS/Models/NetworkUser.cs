using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ProgettoPDS
{
    public class NetworkUser
    {
        private readonly Object locker = new Object();
        private static string _username;
        private static string _ipaddress;
        private static string _imagepath;
        private static string _mac;
        private DateTime _timestamp;


        public NetworkUser() { }

        public NetworkUser(string data)
            //JSON CONSTRUCTOR
        {
            JObject o = JObject.Parse(data);
            Username = (string)o["Name"];
            MACAddress = (string)o["MACAddress"];
            bool defimage = (bool)o["DefaultImage"];
            string time= ((string)o["Timestamp"]);
            TimeStamp = DateTime.Parse(time);
            //TODO: if(defimage)-->immagine default
            //          else richiesta nuova immagine

        }

        public string Username
        {
            get { lock (locker)   {  return _username;  }  }
            set { lock (locker)   { _username = value;  }  }
        }

        public string Ipaddress
        {
            get { lock (locker)   {  return _ipaddress; }  }
            set { lock (locker)   { _ipaddress = value; }  }
        }

        public string Imagepath
        {
            get { lock (locker)  {  return _imagepath;  }  }
            set { lock (locker)  { _imagepath = value;  }  }
        }

        public string MACAddress
        {
            get { lock (locker)  { return _mac;         }  }
            set { lock (locker)  { _mac = value;        }  }
        }

        public DateTime TimeStamp
        {
            get { lock (locker) { return _timestamp;    }  }
            set { lock (locker) { _timestamp = value;   }  }
        }


        ~NetworkUser()
        {
            if (File.Exists(_imagepath))
            {
                File.Delete(_imagepath);
            }

        }
    }
}
