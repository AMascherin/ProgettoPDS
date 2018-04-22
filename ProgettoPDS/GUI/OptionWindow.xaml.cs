﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;



namespace ProgettoPDS
{
    /// <summary>
    /// Logica di interazione per OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        UserConfiguration cfg = new UserConfiguration();

        public OptionWindow(bool test) { //TODO: Sistemare la logica della creazione delle window con costruttori appropriati
            InitializeComponent();
            contenitore.MaxHeight = 400;
            contenitore.MaxWidth = 600;
            textboxpercorso.Opacity = 50;
            bottonesceglifile.Opacity = 50;
            textboxpercorso.IsEnabled = false;
            bottonesceglifile.IsEnabled = false;
            checkboxstato.IsChecked = false;
            labelutente.Content = "Username";
        }

        public OptionWindow()
        {
            InitializeComponent();
            contenitore.MaxHeight = 400;
            contenitore.MaxWidth = 600;
            textboxpercorso.Opacity = 50;
            bottonesceglifile.Opacity = 50;
            textboxpercorso.IsEnabled = false;
            bottonesceglifile.IsEnabled = false;

            checkboxstato.IsChecked = !(cfg.PrivacyFlag);


            if (!(File.Exists(cfg.ImgPath))) //Se l'immagine scelta dall'utente non è più disponibile....
            {
                cfg.SetDefaultPath(); //TODO: Controllare che le immagini di default non vengano cancellate
            }

            // immagineprofilo2. = new BitmapImage(new Uri(cfg.ImgPath));
            

            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(cfg.ImgPath));
            immagineprofilo2.Fill = myBrush;
            immagineprofilo2.Stretch = Stretch.Fill;


            labelutente.Content = cfg.Username;
        }



        private void bottonesalva_Click(object sender, RoutedEventArgs e)
        {
            if (textboxutente.Text != "")
            {
                labelutente.Content = textboxutente.Text;
                cfg.Username = textboxutente.Text;
                cfg.DumpConfiguration();
            }
        }

        private void bottonesceglifile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString() == "Ok") textboxpercorso.Text = dialog.FileName;
        }

        private void bottoneimmagine_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter =
            "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";
            fileDialog.FilterIndex = 1;

            fileDialog.ShowDialog();
            if (File.Exists(fileDialog.FileName))
            {
                cfg.ImgPath = fileDialog.FileName;
                cfg.DumpConfiguration();
                //immagineprofilo2.Source = new BitmapImage(new Uri(fileDialog.FileName));
                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource =
                    new BitmapImage(new Uri(cfg.ImgPath));
                immagineprofilo2.Fill = myBrush;

                immagineprofilo2.Stretch = Stretch.Fill;
            }
        }

        private void textboxutente_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {

                labelutente.Content = textboxutente.Text;

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((cfg.Username == null)) {
                string msg = "Scegliere un nome utente per completare la configurazione. La mancata scelta di un nome utente comporterà la chiusura dell'applicazione";
                var result= System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel) {
                    e.Cancel = true;
                }
            }

        }

        private void DownloadCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            textboxpercorso.Opacity = 100;
            textboxpercorso.IsEnabled = true;
            bottonesceglifile.Opacity = 100;
            bottonesceglifile.IsEnabled = true;
        }

        private void DownloadCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxpercorso.Opacity = 50;
            textboxpercorso.IsEnabled = false;
            bottonesceglifile.Opacity = 50;
            bottonesceglifile.IsEnabled = false;
        }

        private void PrivacyCheckBox_Checked(object sender, RoutedEventArgs e) //Passaggio alla modalità pubblica
        {
            cfg.PrivacyFlag = false;
        //    cfg.DumpConfiguration(); TODO: Fare il dump solo alla chiusura
        }

        private void PrivacyCheckBox_Unchecked(object sender, RoutedEventArgs e) //Passaggio alla modalità privata
        {
            cfg.PrivacyFlag = true;
        //    cfg.DumpConfiguration();
        }
    }
}
