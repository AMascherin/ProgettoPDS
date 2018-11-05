using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.IO;
using Hardcodet.Wpf.TaskbarNotification;

namespace ProgettoPDS
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                Process.GetCurrentProcess().Kill();
            }

            base.OnStartup(e);

            //Non vogliamo che l'applicazione termini quando una finestra venga chiusa
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            CleanupIcons();

            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string filepath = Path.GetFullPath(Path.Combine(currentfolder, @"..\..\..\RightClickHandlerApplication\bin\Debug\RightClickHandlerApplication.exe"));
            filepath += " %0";

            try
            {
                RightClickManager.Register("*", "ApplicazionePds", "Condividi (PDS)", filepath);
                RightClickManager.Register("Directory", "ApplicazionePds", "Condividi (PDS)", filepath);
            }
            catch (System.UnauthorizedAccessException)
            {
                System.Windows.MessageBox.Show("Esegui il programma in modalità amministratore per assicurare un corretto funzionamento");
            }
            
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

             MainHub mainhub = new MainHub();
             mainhub.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        private void CleanupIcons() {
            string currentfolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            currentfolder += @"\Media\Icons\";

            DirectoryInfo di = new DirectoryInfo(currentfolder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

    }
}
