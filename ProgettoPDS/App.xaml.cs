using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;

namespace ProgettoPDS
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
             //MainHub mainhub = new MainHub();
            // mainhub.Initialize();
             string msg = "Hello World";
            //TCPSender _sender = new TCPSender("192.168.1.105");
            //_sender.SendData(msg);


                TCPReceiver test = new TCPReceiver();
                test.ReceiveData();



            //          var mw = new OptionWindow();
            //          mw.Show();
        }
    }
}
