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
using System.IO;
using Path = System.IO.Path;

namespace ProgettoPDS.GUI
{
    /// <summary>
    /// Logica di interazione per ProgressoInvio.xaml
    /// </summary>
    public partial class ProgressoInvio : Window
    {

    

        UserConfiguration cfg = new UserConfiguration();

        List<Button> listabutton = new List<Button>();
        List<Image> listaimage = new List<Image>();
        List<Grid> listagriglie = new List<Grid>();
        List<Grid> listagriglieesterne = new List<Grid>();

        


        public ProgressoInvio()
        {

            InitializeComponent();
            generaGriglia(NetworkUserManager.userlist[0], "file di prova");

        }

        void generaGriglia(NetworkUser user, string nomefile)
        // FUNZIONE CHE CREA LA GRIGLIA DI UTENTI CONNESSI
        {
            Grid griglia = new Grid();
            Grid grigliaInterna = new Grid();
            griglia.Background = Brushes.AliceBlue;
            System.Windows.Thickness thickGrid = new Thickness(2, 2, 2, 2);
            griglia.Margin = thickGrid;
            griglia.Height = 60;
            griglia.Width = 930;
            grigliaInterna.Background = Brushes.White;
            grigliaInterna.Height = 60;
            grigliaInterna.Width = 300;
            grigliaInterna.Background = Brushes.AliceBlue;
            
            

            Image image = new Image();
            image.Height = 40;
            image.Width = 47;
            System.Windows.Thickness thick = new Thickness(21, 10, 672, 10);
            image.Margin = thick;



            if (user.DefaultImage)
            {

                BitmapImage imagebitmap = new BitmapImage(new Uri(cfg.ImgPath));
                image.Source = imagebitmap;
                image.Stretch = Stretch.Fill;

            }
            else
            {

                BitmapImage imagebitmap = new BitmapImage(new Uri(user.Imagepath, UriKind.Absolute));
                image.Source = imagebitmap;
                image.Stretch = Stretch.Fill;
                
            }



            image.Stretch = Stretch.Fill;

            ProgressBar progressBar = new ProgressBar();
            progressBar.Height = 10;
            Label label = new Label();
            System.Windows.Thickness thick2 = new Thickness(0, 20, 0, 0);
            progressBar.Margin = thick2;
            label.Content = nomefile; //nome del file/cartella che si sta inviando
            label.FontSize = 15;

            grigliaInterna.VerticalAlignment = VerticalAlignment.Center;


            Button elimina = new Button();
            elimina.Width = 40;
            elimina.Height = 30;
            System.Windows.Thickness thick3 = new Thickness(600, 0, 0, 0);
            elimina.Margin = thick3;
            Style style = this.FindResource("MyButton") as Style;
            elimina.Style = style;

            var projectPath = System.IO.Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            projectPath += "\\Media\\cestino_icona.png";


            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(projectPath, UriKind.Absolute));
            brush.Stretch = Stretch.Fill;

            elimina.Background = brush;


            griglia.Children.Add(image);
            grigliaInterna.Children.Add(label);
            grigliaInterna.Children.Add(progressBar);

            griglia.Children.Add(grigliaInterna);
            griglia.Children.Add(elimina);

            listaimage.Add(image);
            listagriglie.Add(grigliaInterna);
            listabutton.Add(elimina);
            listagriglieesterne.Add(griglia);


            elimina.Click += OnClickCancelButton;
 
            stack.Children.Add(griglia);

        }

        public void OnClickCancelButton(Object sender, RoutedEventArgs e) {

            Button obj = sender as Button;
            obj.Opacity = 100;
            
            for (int i = 0; i < listabutton.Count; i++)
            {
                if (obj == listabutton[i])
                {

                    listagriglieesterne[i].Children.Remove(listaimage[i]);
                    listagriglieesterne[i].Children.Remove(listagriglie[i]);
                    listagriglieesterne[i].Children.Remove(listabutton[i]);
                    stack.Children.Remove(listagriglieesterne[i]);
                    

                }    //funzione che seleziona la checkbox e aggiunge l'utente corrispettivo alla lista di utenti a cui inviare
            }



        }
    }


}
