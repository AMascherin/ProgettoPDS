using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ProgettoPDS.Models
{

    //Collezione dei TCP sender al momento attivi
    public static class ActiveTCPSenderManager 
    {
        public static List<TCPSender> ActiveTcpSender { get; set; } = new List<TCPSender>();

        public static ObservableCollection<TCPSender> TcpSenderCollection = new ObservableCollection<TCPSender>();

        public static void AddActiveSender(TCPSender sender) {
            ActiveTcpSender.Add(sender);
        }

        public static void RemoveSender(TCPSender sender) {
            ActiveTcpSender.Remove(sender);
        }

    }
}
