using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Hub //TODO: gestire la responsività con gli eventi(tcp client, interfaccia grafica, cambio della flag della privacy
    {
        private List<NetworkUser> userlist;
        private UserConfiguration uc;

        public Hub() {

            userlist = new List<NetworkUser>();
            uc = new UserConfiguration();
        
        }

        public void Initialize() { //fa la parte di startup

            uc.LoadConfiguration();
            //TODO: creare un thread per UDPsender, UDPlistener
            this.TCPServerStartup();
        }

        private void TCPServerStartup()
        {
            throw new NotImplementedException();
        
        }



    }











}
