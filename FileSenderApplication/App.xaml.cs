using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;

namespace FileSenderApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //TODO1: Lanciare Progetto PDS. Se non esistente segnalare all'utente che c'è un problema con l'installazione
            //TODO2: Leggere il file di configurazione. Se non presente chiudere l'applicazione

           // handler = new ConsoleEventDelegate(ConsoleEventCallback);
           // SetConsoleCtrlHandler(handler, true);

            var _invio = new InvioFile();
            _invio.Show();

        }

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        static bool ConsoleEventCallback(int eventType)
        {
            Console.WriteLine("eventtype " + eventType);
            System.Windows.MessageBox.Show(eventType.ToString());
            if (eventType == 2)
            {
                Console.WriteLine("Console window closing, death imminent");
            }
            return false;
        }

    }
}
