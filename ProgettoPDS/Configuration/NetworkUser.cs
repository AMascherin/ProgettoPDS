using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProgettoPDS
{
    class NetworkUser
    {
        private readonly Object locker = new Object();
        private string _username;
        private string _ipaddress;
        private string _imagepath;
        private string _mac;
        private DateTime _timestamp;
        
       

        public string Username{
            get
            {
                lock (locker)
                {
                    return _username;
                }
            }

            set
            {
                lock (locker)
                {
                    _username = value;
                }
            }
        }

        public string Ipaddress
        {
            get
            {
                lock (locker)
                {
                    return _ipaddress;
                }
            }

            set
            {
                lock (locker)
                {
                    _ipaddress = value;
                }
            }
        }

        public string Imagepath
        {
            get
            {
                lock (locker)
                {
                    return _imagepath;
                }
            }

            set
            {
                lock (locker)
                {
                    _imagepath = value;
                }
            }
        }


        public string Mac
        {
            get
            {
                lock (locker)
                {
                    return _mac;
                }
            }

            set
            {
                lock (locker)
                {
                    _mac = value;
                }
            }
        }

        public DateTime Timestamp
        {
            get
            {
                lock (locker)
                {
                    return _timestamp;
                }
            }

            set
            {
                lock (locker)
                {
                    _timestamp = value;
                }
            }
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
