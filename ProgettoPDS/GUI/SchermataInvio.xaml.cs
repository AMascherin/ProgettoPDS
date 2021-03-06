﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using ProgettoPDS.GUI;

namespace ProgettoPDS
{
    /// <summary>
    /// Interaction logic for SchermataInvio.xaml
    /// </summary>
    public partial class SchermataInvio : Window
    {
        string pathToFile;

        List<Label> listalabel = new List<Label>();

        List<Image> listaimmagini = new List<Image>();

        List<TCPSender> listaSender = new List<TCPSender>();

        List<CheckBox> listacheck = new List<CheckBox>();

        List<NetworkUser> listauserinvio = new List<NetworkUser>(); //Utenti selezionati a cui inviare i file
        
        List<NetworkUser> listauser = new List<NetworkUser>(); //Tutti gli utenti presenti in rete e con profilo pubblico

        ProgressBar prog = new ProgressBar();

        UserConfiguration cfg = new UserConfiguration();


        public SchermataInvio(string filepath, List<NetworkUser> listauser)
        {
            
            this.listauser = listauser;
            InitializeComponent();
            pathToFile = filepath;
            
            foreach (NetworkUser user in listauser)
            {

                generaGriglia(user, listalabel, listaimmagini, listacheck);

            }

        }

        void generaGriglia(NetworkUser user, List<Label> listalabel, List<Image> listaimmagini, List<CheckBox> listacheck)
        // FUNZIONE CHE CREA LA GRIGLIA DI UTENTI CONNESSI
        {
            Grid griglia = new Grid();
            griglia.Background = Brushes.White;
            griglia.Height = 60;
            griglia.Width = 740;
            Image image = new Image();
            image.Height = 40;
            image.Width = 47;
            System.Windows.Thickness thick = new Thickness(21, 10, 672, 10);
            image.Margin = thick;
            
            if(user.DefaultImage)
            {
                BitmapImage imagebitmap = new BitmapImage(new Uri(cfg.ImgPath));
                image.Source = imagebitmap;
                image.Stretch = Stretch.Fill;
                listaimmagini.Add(image);
            }
            else
            {
                BitmapImage imagebitmap = new BitmapImage(new Uri(user.Imagepath, UriKind.Absolute));
                image.Source = imagebitmap;
                image.Stretch = Stretch.Fill;
                listaimmagini.Add(image);
            }
           
            
            image.Stretch = Stretch.Fill;
            listaimmagini.Add(image);
            Label label = new Label();
            System.Windows.Thickness thick2 = new Thickness(134, 20, 473, 10);
            label.Margin = thick2;
            label.Content = user.Username;
            label.FontSize = 15;
            listalabel.Add(label);
            CheckBox cb = new CheckBox();
            cb.Opacity = 0;
            cb.Height = 60;
            cb.Width = 770;
            cb.MouseEnter += CheckBox_MouseEnter;
            cb.MouseLeave += cb_MouseLeave;
            cb.Checked += cb_Checked;
            cb.Unchecked += cb_Unchecked;
            cb.IsChecked = false;
            Style style = this.FindResource("Check") as Style;
            cb.Style = style;
            listacheck.Add(cb);
            griglia.Children.Add(image);
            griglia.Children.Add(label);
            griglia.Children.Add(cb);
            stack.Children.Add(griglia);
        }

        //funzione che serve a colorare la checkbox quando gli si passa sopra il mouse
        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            CheckBox obj = sender as CheckBox;
            obj.Opacity = 100;                                     
        }

        //funzione che serve a decolorare la checkbox quando il mouse non è più sopra di essa
        private void cb_MouseLeave(object sender, MouseEventArgs e)
        {
            CheckBox obj = sender as CheckBox;            
            obj.Opacity = 0;
        }

        //funzione che seleziona la checkbox e aggiunge l'utente corrispettivo alla lista di utenti a cui inviare
        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox obj = sender as CheckBox;
            obj.Opacity = 100;
            obj.MouseLeave -= cb_MouseLeave;
            for (int i = 0; i < listacheck.Count; i++)
            {
                if (obj == listacheck[i]) listauserinvio.Add(listauser[i]);   
            }
            stampaListaInvio();
        }

        //funzione che deseleziona la checkbox e rimuove l'utente corrispettivo dalla lista di utenti a cui inviare
        private void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox obj = sender as CheckBox;
            obj.Opacity = 0;
            obj.MouseLeave += cb_MouseLeave;
            for (int i = 0; i < listacheck.Count; i++)
            {                                                                 
                if (obj == listacheck[i]) listauserinvio.Remove(listauser[i]);
            }
            stampaListaInvio();
        }

        //funzione di debug che serve a stampare la lista di utenti a cui inviare
        private void stampaListaInvio()
        {
            for (int i = 0; i < listauserinvio.Count; i++)
            {
                Console.WriteLine(listauserinvio[i].Username);                  
            }
        }

        //fa comparire la progbar quando si clicca sul bottone invia
        private void OnBtnClick(object sender, EventArgs e)
        {

            if (listauserinvio.Count != 0) prog.Opacity = 100;   

        }

        private void bottonecanc_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listauserinvio.Count; i++)
            {
                listauserinvio.Remove(listauserinvio[i]);
                //METODO DELLA X ROSSA CHE DESELEZIONA TUTTE LE PERSONE A CUI SI ERA SCELTO DI INVIARE
            }
            for (int i = 0; i < listacheck.Count; i++)
            {
                if (listacheck[i].IsChecked == true) listacheck[i].IsChecked = false;
            }
        }

        //Attivazione di una connessione TCP per ogni utente destinatario dei file
        private void bottoneinvia_Click(object sender, RoutedEventArgs e)
        {
            List<String> filestosend = new List<string>();
            filestosend.Add(pathToFile);
            foreach (NetworkUser user in listauserinvio) {
                TCPSender tcpsender = new TCPSender(user);
                listaSender.Add(tcpsender);
                Thread TCPSenderThread = new Thread(() => tcpsender.handleFileSend(filestosend));
                TCPSenderThread.Start();
                Models.ActiveTCPSenderManager.TcpSenderCollection.Add((TCPSender)tcpsender);
                
                if (!IsWindowOpen<StatusInvio>())
                {
                    StatusInvio status = new StatusInvio();
                    status.Show();
                }
            }
            
            Close();
        }

        //Funzione per controllare se una generica finestra T è al momento visibile
        private static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }
    }
}
