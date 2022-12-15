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
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Drawing;

namespace TVS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server server;
        Client client;
        public MainWindow()
        {
            server = new Server();
            InitializeComponent();
            SearchOffline();


        }


        private void ListOfClient()
        {
            SearchOffline();
            string OnlineMembersList = "List of online members:\r\n";
            for (int i = 0; i < server.Connections.Count; i++)
            {

                if (server.Connections[i].Accept.Connected)
                {
                    int Num = 0;
                    IPEndPoint TargetIPEndPoint = server.Connections[i].Accept.RemoteEndPoint as IPEndPoint;
                    int port = TargetIPEndPoint.Port;
                    IPAddress Ip = TargetIPEndPoint.Address;
                    Num = i + 1;
                    OnlineMembersList += "Client Number " + Num + "\r\nAddress: " + Ip + "\r\nPort: " + port + "\r\n\r\n";
                }
            }
            online_list.Text = OnlineMembersList;
        }


        private void SetIP_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            server.SetEndPoint(IP.Text, Port.Text);
            status.Text += "\r\n" + server.GetStatusReport();
        }


        private void SetListenNum_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            server.SetListenNumber(ListenNum.Text);
            status.Text += "\r\n" + server.GetStatusReport();
        }


        private void StartAccepting_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            server.StartListening();
            status.Text += "\r\n" + server.GetStatusReport();

        }


        private void FindClient_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            bool Exist = false;
            client = server.FindClient(ClientIP.Text, ClientPort.Text, out Exist);
            status.Text += "\r\n" + server.GetStatusReport();
            if (Exist)
            {
                ClientManagement ClientInfo = new ClientManagement();
                ClientInfo.SetClient(client);
                ClientInfo.Show();
            }
        }


        private void StopListening_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            server.StopListening();
            status.Text += "\r\n" + server.GetStatusReport();
        }


        private void RefreshClient1_Click(object sender, RoutedEventArgs e)
        {
            //theared--------------------list
            SearchOffline();
            ListOfClient();
        }


        private void SendMessageToAll_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            server.SendMessageToAll(MessageToAll.Text);
            status.Text += "\r\n" + server.GetStatusReport();
            MessageToAll.Text = null;
        }


        private void RefreshNetworkTraffic_Click(object sender, RoutedEventArgs e)
        {
            SearchOffline();
            NetworkTraffic.Text = (server.TrafficCalculating()).ToString();
            status.Text += "\r\n" + server.GetStatusReport();
        }

        private void ServerOff_Click(object sender, RoutedEventArgs e)
        {
            server.ServerShutDown();
            status.Text += "\r\n" + server.GetStatusReport();
        }

        private void SearchOffline()
        {
            for (int i = 0; i < server.Connections.Count; i++)
            {
                if (server.Connections[i].Accept.Connected != true)
                {
                    server.Connections.Remove(server.Connections[i]);
                }
            }
        }

        /*public void ststusWriter(string Report)
        {
            status.Text += "\r\n" + Report;
        }*/


    }

}
