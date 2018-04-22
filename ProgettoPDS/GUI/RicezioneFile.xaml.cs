using System;
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
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProgettoPDS.GUI
{
    /// <summary>
    /// Interaction logic for RicezioneFile.xaml
    /// </summary>
    public partial class RicezioneFile : Window
    {

        List<Label> listalabel = new List<Label>();


        public RicezioneFile()
        {
            InitializeComponent();
       /*     generaGriglia(listalabel, "testo", "txt");
            generaGriglia(listalabel, "testo", "txt");
            generaGriglia(listalabel, "testo", "txt");
            generaGriglia(listalabel, "testo", "txt");
            generaGriglia(listalabel, "testo", "txt");
            generaGriglia(listalabel, "testo", "txt");*/

        }

        public RicezioneFile(List<String> filetoreceive) { //TODO:finire
            InitializeComponent();
            foreach (String file in filetoreceive) {
                generaGriglia(listalabel, file, file, file);
            }

        }

        private void bottonesceglifile_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString() == "Ok") textboxpercorso.Text = dialog.FileName;

        }



        void generaGriglia(List<Label> listalabel, string nomefile, string estensione, string size)
        // FUNZIONE CHE CREA LA GRIGLIA DI UTENTI CONNESSI
        {

            Grid griglia = new Grid();
            griglia.Height = 35;
            griglia.Width = 740;
            Label label = new Label();
            //System.Windows.Thickness thick2 = new Thickness(134, 20, 473, 10);
            //label.Margin = thick2;
            label.Content = nomefile + "." + estensione + " " + size;
            label.Height = 30;
            label.Width = 750;
            label.Background = new SolidColorBrush(Colors.White);
            Style style = this.FindResource("LabelStyle") as Style;
            label.Style = style;
            griglia.Children.Add(label);
            stack.Children.Add(griglia);


        }




    }
}
