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

namespace TVS
{
    /// <summary>
    /// Interaction logic for ClientManagment.xaml
    /// </summary>
    public partial class ClientManagement : Window
    {
        Client client;
        public ClientManagement()
        {
            InitializeComponent();
        }


        public void SetClient(object a)
        {
            this.client = (Client)a;
            if (client.Accept.Connected)
            {

                Online.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF28F39F"));
                Online.Text = "   Online";
            }
            else
            {
                Online.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCB0E37"));
                Online.Text = "  Offline";
            }
            ClientIP.Text = client.MyIPAddress.ToString();
            ClientPort.Text = client.MyPort.ToString();
            TrafficAmount.Text = client.Accept.Available.ToString();
        }


        private void RefreshTrafficAmount_Click(object sender, RoutedEventArgs e)
        {
            int Traffic = client.Accept.Available;
            TrafficAmount.Text = Traffic.ToString();
        }


        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            Report.Text = client.GetReporter();
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            client.CloseClient(TimeOut.Text);
            Report.Text = client.GetReporter();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            client.ServerMessageSender(ServerMessage.Text);
            Report.Text = client.GetReporter();
            ServerMessage.Text = null;
        }
    }
}
