using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;



namespace ProgettoPDS
{
    /// <summary>
    /// Logica di interazione per OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        UserConfiguration cfg = new UserConfiguration();
        private string username;
        private string imgpath;
        private bool automaticDownloadAcceptance;
        private bool privacyFlag;
        private bool defaultDownloadPath;
        private string downloadPath;
        
        public OptionWindow()
        {
            InitializeComponent();

            //Inizializzazione generale UI
            contenitore.MaxHeight = 400;
            contenitore.MaxWidth = 600;
            textboxpercorso.Opacity = 50;
            bottonesceglifile.Opacity = 50;
            
            //Immagine profilo
            if (!(File.Exists(cfg.ImgPath))) //Se l'immagine scelta dall'utente non è più disponibile....
            {
                cfg.SetDefaultPath(); 
            }
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri(cfg.ImgPath));
            immagineprofilo2.Fill = myBrush;
            immagineprofilo2.Stretch = Stretch.Fill;
            imgpath = cfg.ImgPath;

            //Set label utente con username
            labelutente.Content = (cfg.Username != null) ? cfg.Username : "Username";
            username = cfg.Username;

            //Set "automatically accept"           
            checkboxdownload.IsChecked = !cfg.AutomaticDownloadAcceptance;
            automaticDownloadAcceptance = cfg.AutomaticDownloadAcceptance;

            //Bottone Online/Offline
            checkboxstato.IsChecked = !(cfg.PrivacyFlag);
            privacyFlag = cfg.PrivacyFlag;

            //Choose Download Path
            defaultDownloadPath = cfg.DefaultDownloadPath;
            checkboxdownloadpath.IsChecked = defaultDownloadPath;
            if (defaultDownloadPath)
            {
                textboxpercorso.IsEnabled = true;
                textboxpercorso.Text = cfg.DefaultDownloadPathString;
                downloadPath = cfg.DefaultDownloadPathString;
                bottonesceglifile.IsEnabled = true;
                
            }
            else
            {
                textboxpercorso.IsEnabled = false;
                bottonesceglifile.IsEnabled = false;
            }         
        }

        //Salvataggio dei dati nella struttura dati e su disco
        private void bottonesalva_Click(object sender, RoutedEventArgs e)
        {
            if (textboxutente.Text != "")
            {
                labelutente.Content = textboxutente.Text;
                username = textboxutente.Text;
            }

            if (username != null && username != "") cfg.Username = username;
            cfg.AutomaticDownloadAcceptance = automaticDownloadAcceptance;
            cfg.DefaultDownloadPath = defaultDownloadPath;
            cfg.DefaultDownloadPathString = (cfg.DefaultDownloadPath)? downloadPath : null;            
            cfg.PrivacyFlag = privacyFlag;
            if (!imgpath.Equals(cfg.ImgPath)) cfg.ImgPath = imgpath;
            cfg.DumpConfiguration();
        }

        //Bottone per aprire la File Dialog di windows e scegliere una cartella di default per i download
        private void bottonesceglifile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString() == "Ok")
            {
                downloadPath = dialog.FileName;
                textboxpercorso.Text = dialog.FileName;
            }
        }

        //Bottone per cambiare l'immagine del profilo
        private void bottoneimmagine_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.ShowDialog();

            if (File.Exists(fileDialog.FileName))
            {
                imgpath = fileDialog.FileName;
                ImageBrush myBrush = new ImageBrush();
                //myBrush.ImageSource = new BitmapImage(new Uri(cfg.ImgPath));
                myBrush.ImageSource = new BitmapImage(new Uri(imgpath));

                immagineprofilo2.Fill = myBrush;
                immagineprofilo2.Stretch = Stretch.Fill;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isConfigurationChanged())
            {
                string msg = "Vuoi uscire senza salvare le modifiche effettuate?";
                var result = System.Windows.MessageBox.Show(msg, "Salvare le modifiche", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) {
                    e.Cancel = true;
                }
            }

            if ((cfg.Username == null))
            {
                string msg = "Scegliere un nome utente per completare la configurazione. La mancata scelta di un nome utente comporterà la chiusura dell'applicazione";
                var result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            if(defaultDownloadPath && downloadPath == null)
            {
                string msg = "E' necessario impostare un path di download se si vuole utilizzare un path di default";
                var result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Cancel = true;
            }
        }

        private bool isConfigurationChanged() {

            if (username != cfg.Username) return true;
            else if (privacyFlag != cfg.PrivacyFlag) return true;
            else if (automaticDownloadAcceptance != cfg.AutomaticDownloadAcceptance) return true;
            else if(imgpath != cfg.ImgPath) return true;
            else if(defaultDownloadPath != cfg.DefaultDownloadPath) return true;
            else if(downloadPath != cfg.DefaultDownloadPathString) return true;
            else return false;
        }

        private void DownloadCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            textboxpercorso.Opacity = 100;
            textboxpercorso.IsEnabled = true;
            bottonesceglifile.Opacity = 100;
            bottonesceglifile.IsEnabled = true;

            defaultDownloadPath = true;
          
        }

        private void DownloadCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxpercorso.Opacity = 50;
            textboxpercorso.IsEnabled = false;
            bottonesceglifile.Opacity = 50;
            bottonesceglifile.IsEnabled = false;


            textboxpercorso.Text = null;
            downloadPath = null;
            defaultDownloadPath = false;
        }

        private void PrivacyCheckBox_Checked(object sender, RoutedEventArgs e) //Passaggio alla modalità pubblica
        {
            privacyFlag = false;
        }

        private void PrivacyCheckBox_Unchecked(object sender, RoutedEventArgs e) //Passaggio alla modalità privata
        {
            privacyFlag = true;
        }

        private void checkboxdownload_Checked(object sender, RoutedEventArgs e)
        {
            automaticDownloadAcceptance = false;
        }

        private void checkboxdownload_Unchecked(object sender, RoutedEventArgs e)
        {
            automaticDownloadAcceptance = true;
        }

        private void textboxpercorso_TextChanged(object sender, TextChangedEventArgs e)
        {

            downloadPath = textboxpercorso.Text;

        }
    }
}
