using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ProgettoPDS
{
    public class NetworkUser
    {
        private readonly Object locker = new Object();
        private string _username;
        private string _ipaddress;
        private string _imagepath;
        private string _mac;
        private bool _defaultImage;
        private DateTime _timestamp;
        private DateTime _imagetimestamp;

        public NetworkUser() { }

        public NetworkUser(string data)
            //JSON CONSTRUCTOR
        {
            JObject o = JObject.Parse(data);
            Username = (string)o["Name"];
            MACAddress = (string)o["MACAddress"];
            DefaultImage = (bool)o["DefaultImage"];
            string time= ((string)o["Packet Timestamp"]);
            TimeStamp = DateTime.Parse(time);

            ImageTimeStamp = DateTime.Parse((string)o["Image change timestamp"]);
            
        }

        public bool DefaultImage {
            get { lock (locker) { return _defaultImage; } }
            set { lock (locker) { _defaultImage = value; } }
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
        public DateTime ImageTimeStamp
        {
            get { lock (locker) { return _imagetimestamp; } }
            set { lock (locker) { _imagetimestamp = value; } }
        }

        ~NetworkUser()
        {
        //    if (File.Exists(_imagepath))
        //    {
        //        File.Delete(_imagepath);
        //    }

        }
    }
}
