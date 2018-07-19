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
using System.Threading;

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
            // MainHub mainhub = new MainHub();
            // mainhub.Initialize();

            /* string msg = "Hello World";
            TCPSender _sender = new TCPSender("192.168.1.175");
            _sender.SendData(msg);*/

            //         string path = @"C:\Users\Alessandro Mascherin\Desktop\The Wizard\Video\The_Lizard_Movie.mp4";
            //  string path = @"C:\Users\fabyf\Desktop\leaves.jpg";
            //  string path2 = @"C:\Users\fabyf\Desktop\caso.wav";
            string path = @"C:\Users\fabyf\Desktop\INGLESE\";

       /*     try
            {
                TCPSender _sender = new TCPSender("192.168.43.86");
                List<String> files = new List<string>();
                files.Add(path);
                _sender.handleFileSend(files);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return;
            }*/
            
         
           TCPServer test = new TCPServer();
            Thread _tcprecThread = new Thread(test.StartListener);
            _tcprecThread.Start(); 

            //string path = @"C:\Users\fabyf\Desktop\leaves.jpg";
         /*   string path = @"C:\Users\Alessandro Mascherin\Pictures\wnlYdQw.png";
            TCPSender sender = new TCPSender("192.168.43.138");
            List<String> files = new List<string>();
            files.Add(path);
            sender.handleFileSend(files);*/
            
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
            //var rf = new OptionWindow();
            //rf.Show();

        }
    }
}
