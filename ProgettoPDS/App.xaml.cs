using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;
using ProgettoPDS.GUI;

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

            /* string msg = "Hello World";
            TCPSender _sender = new TCPSender("192.168.1.105");
            _sender.SendData(msg);*/

            //TCPServer test = new TCPServer();
            //test.ReceiveData();

            /* string path = @"C:\Users\fabyf\Desktop\leaves.jpg";
             TCPSender _sender = new TCPSender("192.168.1.105");
             _sender.SendData(path);*/

            //TCPServer test = new TCPServer();
            //test.ReceiveData();

            /*  string pathToObj = @"C:\Users\Alessandro\Desktop\image.jpg";
              byte[] bytesToSend = File.ReadAllBytes(pathToObj);
              string fileName = "Pippo.jpg";
              try
              {
                  using (StreamWriter writer = new StreamWriter(fileName, true))
                  {

                      writer.BaseStream.Write(bytesToSend,0,bytesToSend.Length);
                  }
              }
              catch (DirectoryNotFoundException ex)
              {
                  Console.WriteLine("DirectoryNotFoundException: {0}", ex);
              }

      */


            //   var mw = new OptionWindow();
            //   mw.Show();
            var rf = new RicezioneFile();
              rf.Show();

        }
    }
}
