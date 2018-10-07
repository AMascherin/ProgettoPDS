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

namespace ProgettoPDS.GUI
{
    /// <summary>
    /// Logica di interazione per StatusInvio.xaml
    /// </summary>
    public partial class StatusInvio : Window
    {
        public StatusInvio(List<TCPSender> items)
        {
            InitializeComponent();
            lvDataBinding.ItemsSource = items;
        }

        public StatusInvio() {
            InitializeComponent();
            lvDataBinding.ItemsSource = Models.ActiveTCPSenderManager.TcpSenderCollection;
        }
    }
}
