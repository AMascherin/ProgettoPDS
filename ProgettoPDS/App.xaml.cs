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
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                Process.GetCurrentProcess().Kill();
            }

            base.OnStartup(e);

            //Non vogliamo che l'applicazione termini quando una finestra venga chiusa
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;


            //RightClickManager.Register("fileType", "shellKeyName", "menuText", "menuCommand");
            string filepath = @"C:\Users\Alessandro Mascherin\Source\Repos\ProgettoPDS\RightClickHandlerApplication\bin\Debug\RightClickHandlerApplication.exe %0 %1";
            RightClickManager.Register("*", "ApplicazionePds", "Condividi (PDS)", filepath);
            RightClickManager.Register("Directory", "ApplicazionePds", "Condividi (PDS)", filepath);

            //SchermataInvio test = new SchermataInvio("test");
            //OptionWindow test = new OptionWindow();

            //test.Show();

             MainHub mainhub = new MainHub();
             mainhub.Initialize();
        }



    }
}
