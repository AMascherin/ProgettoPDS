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
            UserConfiguration cfg = new UserConfiguration();
            try { cfg.LoadConfiguration(); }
            catch(IOException)
            {
                OptionWindow mw1 = new OptionWindow(true);
                mw1.Show();
                string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string fullpath = folder + @"\config.json";
                if (!File.Exists(fullpath))
                {
                    string msg = "Non sono stati inseriti i dati richiesti"; //TODO: sincronizzarsi con la schermata principale
                    System.Windows.MessageBox.Show(msg);
                    return;
                }
            }
           
            var mw = new OptionWindow();
            mw.Show();
        }
    }
}
