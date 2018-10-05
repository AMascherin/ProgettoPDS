using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProgettoPDS.GUI
{
    /// <summary>
    /// Interaction logic for RicezioneFile.xaml
    /// </summary>
    public partial class RicezioneFile : Window
    {

        List<Label> listalabel = new List<Label>();

        public String downloadPath;

        private bool _AcceptDownload;

        public void Reset() {
            downloadPath = null;
            _AcceptDownload = false;
        }

        public bool AcceptDownload
        {
            get
            {
                return _AcceptDownload;
            }
        }

        public RicezioneFile()
        {
            InitializeComponent();

            for (int i = 0; i < 50; i++)
            {
                generaGriglia(listalabel, "testo", "txt", "1200 bytes"+i);
                generaGriglia(listalabel, "testo2", "txt2", "3200 bytes"+i);
            }

            if (App.Current.MainWindow == this)
            {
                App.Current.MainWindow = null;
            }
        }
        

        public RicezioneFile(List<Models.DownloadItemModel> downloadItems) {
            InitializeComponent();
            if (App.Current.MainWindow == this)
            {
                App.Current.MainWindow = null;
            }

            foreach (var item in downloadItems)
            {
                generaGriglia(listalabel, item.OriginalFileName, item.Format, item.Dimension.ToString());
            }
        }

        private void bottonesceglifile_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString() == "Ok") textboxpercorso.Text = dialog.FileName;
            downloadPath = textboxpercorso.Text;

        }



        void generaGriglia(List<Label> listalabel, string nomefile, string estensione, string size)
        // FUNZIONE CHE CREA LA GRIGLIA DI FILE DA RICEVERE
        {

            Grid griglianome = new Grid();
            griglianome.Height = 30;
            griglianome.Width = 230;
            Grid grigliaestensione = new Grid();
            grigliaestensione.Height = 30;
            grigliaestensione.Width = 230;
            Grid grigliadimensione = new Grid();
            grigliadimensione.Height = 30;
            grigliadimensione.Width = 230;
            Label label = new Label();
            Label labelestensione = new Label();
            Label labeldimensione = new Label();


            //System.Windows.Thickness thick2 = new Thickness(134, 20, 473, 10);
            //label.Margin = thick2;
            label.Content = nomefile;
            label.Height = 30;
            label.Width = 250;
            //label.Background = new SolidColorBrush(Colors.White);
            //Style style = this.FindResource("LabelStyle") as Style;
            //labelestensione.Style = style;
            labelestensione.Content = estensione;
            labelestensione.Height = 30;
            labelestensione.Width = 100;
            //labelestensione.Background = new SolidColorBrush(Colors.White);
        
            //labeldimensione.Style = style;
            labeldimensione.Content = size;
            labeldimensione.Height = 30;
            labeldimensione.Width = 130;
            //labeldimensione.Background = new SolidColorBrush(Colors.White);
       
            //labeldimensione.Style = style;
            griglianome.Children.Add(label);
            grigliaestensione.Children.Add(labelestensione);
            grigliadimensione.Children.Add(labeldimensione);
            stackname.Children.Add(griglianome);
            stackdimensione.Children.Add(grigliadimensione);
            stackestensione.Children.Add(grigliaestensione);




        }
        

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (textboxpercorso.Text.Equals("")) System.Windows.MessageBox.Show("Choose a path for the download");
            else _AcceptDownload = true;
            this.Close();
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
