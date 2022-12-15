using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TVS
{
    class Server
    {
        int ServerPort;
        IPAddress ServerIP;
        Socket listenerSocket;
        IPEndPoint ServerEndpoint;
        int listeningNumber;
        int NetworkTraffic = 0;
        bool IsListening = true;
        string Status;
        public List<Client> Connections = new List<Client>();


        public Server()
        {

        }


        public void SetEndPoint(string IP, string Port)
        {
            if (!IPAddress.TryParse(IP, out this.ServerIP))
            {
                SetStatusReport(" Invalid server IP supplied.");
                return;
            }
            IPAddress.TryParse(IP, out this.ServerIP);
            if (!int.TryParse(Port.Trim(), out ServerPort))
            {
                SetStatusReport(" Invalid port number supplied, return.");
                return;
            }
            this.ServerPort = Convert.ToInt32(Port);
            if (ServerPort <= 0 || ServerPort > 65535)
            {
                SetStatusReport(" Port number must be between 0 and 65535.");
                return;
            }
            ServerEndpoint = new IPEndPoint(ServerIP, ServerPort);
            SetStatusReport(" The IP set (" + IP + ":" + Port + ")");
        }


        public void SetListenNumber(string number)
        {
            this.listeningNumber = Convert.ToInt32(number);
            SetStatusReport(" The listening number set (" + number + ")");
        }


        public void SetStatusReport(string Report)
        {
            this.Status = DateTime.Now + Report;
            //Reporter.ststusWriter(Status);
            //Reporter.status.Text += "\r\n" + Status;
        }


        public string GetStatusReport()
        {
            return Status;

        }


        public void StartListening()
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(ServerEndpoint);
            listenerSocket.Listen(listeningNumber);
            SetStatusReport(" Server is listening...");
            IsListening = true;
            listenerSocket.BeginAccept(Accepting, listenerSocket);

        }


        //Ali*********************************************************
        private void Accepting(IAsyncResult ar)
        {
            Socket listenerSocket = (Socket)ar.AsyncState;
            if (!IsListening)
            {
                return;
            }
            Socket Accept = listenerSocket.EndAccept(ar);
            Client NewClient = new Client(Accept);
            NewClient.ReadyToRecive();
            Connections.Add(NewClient);
            //StatusReport(" The client index:" + connections.IndexOf(NewClient));
            Console.WriteLine(" The client index:" + (Connections.IndexOf(NewClient) + 1) + "is connected");
            Console.WriteLine(" The client index:" + NewClient.Accept.RemoteEndPoint.ToString() + "is connected");////////
            listenerSocket.BeginAccept(Accepting, listenerSocket);
            NewClient.FindClient += ClientFinding;
        }


        public struct MessageToClinet
        {
            public byte[] Message;
            public string TargetID;
        }


        private void ClientFinding(object sender, MessageToClinet e)
        {
            Client host = Connections.Find(x => x.Source_IP == e.TargetID);
            host.send(e.Message);
        }


        public Client FindClient(string IP, string Port, out bool Exist)
        {
            IPAddress ClientIP;
            int ClientPort;
            if (!IPAddress.TryParse(IP, out ClientIP))
            {
                SetStatusReport(" Invalid server IP supplied.");
                Exist = false;
                return null;
            }
            IPAddress.TryParse(IP, out ClientIP);
            if (!int.TryParse(Port.Trim(), out ClientPort))
            {
                SetStatusReport(" Invalid port number supplied, return.");
                Exist = false;
                return null;
            }
            ClientPort = Convert.ToInt32(Port);
            if (ServerPort <= 0 || ServerPort > 65535)
            {
                SetStatusReport(" Port number must be between 0 and 65535.");
                Exist = false;
                return null;
            }

            foreach (Client target in Connections)
            {
                IPEndPoint TargetIPEndPoint = target.Accept.RemoteEndPoint as IPEndPoint;
                IPAddress TargetIP = TargetIPEndPoint.Address;
                int TargetPort = TargetIPEndPoint.Port;
                if ((ClientPort == TargetPort) && (ClientIP.Equals(TargetIP)))
                {
                    SetStatusReport(" The client was found");
                    Exist = true;
                    return target;
                }
            }
            SetStatusReport(" The client is not found");
            Exist = false;
            return null;
        }


        public void SendMessageToAll(string Message)
        {
            foreach (Client Target in Connections)
            {
                Target.ServerMessageSender(Message);
                SetStatusReport(" The message: (" + Message + ") send's to all clients.");
            }
        }


        public int TrafficCalculating()
        {
            foreach (Client Target in Connections)
            {
                NetworkTraffic += Target.Accept.Available;
            }
            SetStatusReport(" Server traffic updated.");
            return NetworkTraffic;
        }


        public void StopListening()
        {
            IsListening = false;
            listenerSocket.Close();
            SetStatusReport(" server is stoped listening.");

        }


        public void ServerShutDown()
        {
            StopListening();


            foreach (Client Target in Connections)
            {
                string TimeOut = "0";
                Target.CloseClient(TimeOut);
            }
            SetStatusReport(" Server is shuted down");
        }


    }
}
