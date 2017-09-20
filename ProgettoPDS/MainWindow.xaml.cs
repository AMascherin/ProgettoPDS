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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace ProgettoPDS
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            contenitore.MaxHeight = 319; 
            contenitore.MaxWidth = 517;
            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            textboxpercorso.Opacity = 100;
            textboxpercorso.IsEnabled = true;
            bottonesceglifile.Opacity = 100;
            bottonesceglifile.IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxpercorso.Opacity = 0;
            textboxpercorso.IsEnabled = false;
            bottonesceglifile.Opacity = 0;
            bottonesceglifile.IsEnabled = false;

        }

        private void bottonesalva_Click(object sender, RoutedEventArgs e)
        {
            if (textboxutente.Text != "")
            {
                labelutente.Content = textboxutente.Text;
                textboxutente.Text = "";
            }
        }

        private void bottonesceglifile_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowDialog();
            textboxpercorso.Text = folderDialog.SelectedPath;
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
                immagineprofilo2.Source = new BitmapImage(new Uri(fileDialog.FileName));
                immagineprofilo2.Stretch = Stretch.Fill;
            }
        }
    }
}
