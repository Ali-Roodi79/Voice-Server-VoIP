using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TVS
{
    class Client
    {
        string FileName = "1";
        public Socket Accept;
        long Size;
        int Save = -1;
        byte[] Data;
        int BufferSize = 1024;
        int ReceivedNumber = 0;
        string Password;
        string UserEmail;
        byte[] Receive;
        int Cursor = 0;     
        public string Source_IP;
        Packets.Header Header = new Packets.Header();
        bool IsConnected = true;
        string ServerMessageStr;
        public IPAddress MyIPAddress;
        public int MyPort;
        string Report;
        // Routing function call
        public event EventHandler<Server.MessageToClinet> FindClient;
        /*public string ClientReport
        {
            get
            {
                return ClientReport;///&&IF
            }
            set
            {
                ClientReport += "\r\n" + DateTime.Now + value;
            }

        }*/


        public Client(Socket Accept)
        {
            this.Accept = Accept;
            MyIPAddress = (Accept.RemoteEndPoint as IPEndPoint).Address;
            MyPort = (Accept.RemoteEndPoint as IPEndPoint).Port;
            //ClientReport = (" Just connected to server.");
        }


        public void ReadyToRecive()
        {

            Receive = new byte[1024];
            Accept.BeginReceive(Receive, 0, BufferSize, SocketFlags.None, ReceiveCallBack, -1);

        }


        public void ReceiveCallBack(IAsyncResult ar)
        {

           
            int Remain = (int)ar.AsyncState;
            //Stop socket processing if being closed
            if(!IsConnected)
            {
                return;
            }
            

            try
            {

                ReceivedNumber = Accept.EndReceive(ar);

            }
            catch (Exception e)
            {

                Console.WriteLine(e);

            }
            //Stop socket processing if being disconnected
            if (!Accept.Connected)
            {
                return;
            }


            Packets P = new Packets();
            Packets.DataManagment DataManagement = new Packets.DataManagment();
            Packets.packettype DataType = new Packets.packettype();
            DataManagement = (Packets.DataManagment)P.DeEncapsulation<Packets.DataManagment>(Receive);
            Header = (Packets.Header)P.DeEncapsulation<Packets.Header>(Receive);
            DataType = Header.Type;
            if (Remain == -1)
            {
                // Set routing info************
                Source_IP = Header.SourceID; 
                Size = Header.Size;

                if (DataType != Packets.packettype.Login)
                {
                    for (int i = 0, j = 0; j != 4; i++)
                    {
                        if (DataManagement.Data[i] == Convert.ToByte('$'))  /// Spliting Data from Hedear 
                        {
                            Save = i + 1;
                            j++;

                        }
                    }
                    Data = new byte[1024 - Save];  // Copy data out of data managemet
                    Array.Copy(DataManagement.Data, Save, Data, 0, DataManagement.Data.Length - Save);
                }
            }
            else
            {
                //Here we don't have header and all the received message is data.
                Data = DataManagement.Data.Take<byte>(ReceivedNumber).ToArray();
            }


            switch (DataType)           
            {


                case Packets.packettype.SignUp:
                    SetReport(" Client sent a signup request.");
                    Data = Data.Take<byte>((int)Size).ToArray();
                    UserEmail = Encoding.UTF8.GetString(Data);
                    Email SignupRequest = new Email();
                    Password = Path.GetRandomFileName();
                    SignupRequest.EMAIL(Password, UserEmail);
                    SetReport(" Server sent signup answer back to: " + UserEmail);
                    break;


                case Packets.packettype.Login:
                    SetReport(" Client sent a login request.");
                    Packets.LoginDataManagment loginData = new Packets.LoginDataManagment();
                    Packets.Login login = new Packets.Login();
                    Receive = Receive.Take<byte>(ReceivedNumber).ToArray();
                    loginData = (Packets.LoginDataManagment)P.StringDeEncapulation<Packets.LoginDataManagment>(Receive);
                    login = (Packets.Login)P.StringDeEncapulation<Packets.Login>(Encoding.UTF8.GetBytes( loginData.Data));
                     
                    if ( login.Password == Password)
                    {
                        SetReport(" The login request was successful.");
                        Packets.LoginDataManagment LoginData = new Packets.LoginDataManagment();
                        login.Username = UserEmail;
                        login.Password = Password;
                        //Formating LoginDatamanagement.Header
                        Header.Size = 1;
                        Header.Type = Packets.packettype.Login;
                        Header.SourceID = "Server";
                        Header.TargetID = "Client";
                        //Formating LoginDataManagement.Data
                        LoginData.Data = P.ReturnToString(login);
                        LoginData.Header = P.ReturnToString(Header);
                        //Encapsulating
                        string LoginAnswerStr;
                        LoginAnswerStr = P.ReturnToString(LoginData);
                        byte[] LoginAnswer = Encoding.UTF8.GetBytes(LoginAnswerStr);
                        send(LoginAnswer);
                        SetReport(" Server sent login answer back to client.");
                    }
                    else
                    {
                        SetReport(" The login request was was failed because of wrong password.");
                        Packets.LoginDataManagment LoginData = new Packets.LoginDataManagment();
                        login.Username = UserEmail;
                        login.Password = Password;
                        //Formating LoginDatamanagement.Header
                        Header.Size = 0;
                        Header.Type = Packets.packettype.Login;
                        Header.SourceID = "Server";
                        Header.TargetID = "Client";
                        //Formating LoginDataManagement.Data
                        LoginData.Data = P.ReturnToString(login);
                        LoginData.Header = P.ReturnToString(Header);
                        //Encapsulating
                        string LoginAnswerStr;                       
                        LoginAnswerStr = P.ReturnToString(LoginData);
                        byte[] LoginAnswer = Encoding.UTF8.GetBytes(LoginAnswerStr);
                        send(LoginAnswer);
                        SetReport(" Server sent login answer back to client.");
                    }
                    break;
            }


            if (DataType != Packets.packettype.SignUp && DataType != Packets.packettype.Login && DataType != Packets.packettype.Message)
            {

                AppendAllBytes(@"D:\" + FileName.ToString() + ".mp3", Data);  /// Append to the file

            }


            Cursor += Data.Length; // When Our Data has been finished 


            if (DataType != Packets.packettype.SignUp && DataType != Packets.packettype.Login)
            {
                // Routing to target client
                string Destination = Header.TargetID;
                Server.MessageToClinet MessageSender = new Server.MessageToClinet();
                MessageSender.TargetID = Destination;
                MessageSender.Message = Receive;
                FindClient(this, MessageSender);

            }



            if (Cursor >= Size || Header.Type == Packets.packettype.Login || Header.Type == Packets.packettype.SignUp) //Receiving is finished                                                                                                                         // and Receive New Data
            {
                //Making everything ready for next receive.
                SetReport(" Client sent a " + Size + " bytes" + DataType + " to" + Header.TargetID);
                Cursor = 0;
                Save = -1;
                Receive = new byte[1024];
                //Changing filename for next receive.
                FileName = Path.GetRandomFileName();
                Accept.BeginReceive(Receive, 0, BufferSize, SocketFlags.None, ReceiveCallBack, -1);
            }
            else
            {
                //Continueing receiving current meassage.
                Accept.BeginReceive(Receive, 0, BufferSize, SocketFlags.None, ReceiveCallBack, (int)Size - (int)Cursor);

            }


        }


        public static void AppendAllBytes(string Path, byte[] Data)
        {

            using (var Stream = new FileStream(Path, FileMode.Append))
            {
                Stream.Write(Data, 0, Data.Length);
            }
        }


        public void send(byte[] Send)
        {
            Accept.BeginSend(Send, 0, Send.Length, SocketFlags.None, SendCallback, Accept);
        }


        private void SendCallback(IAsyncResult ar)
        {
            Socket SEND = (Socket)ar.AsyncState;
            int SendingNumber = Accept.EndSend(ar);
        }


        public void ServerMessageSender(string ServerMessageStr)
        {
            this.ServerMessageStr = ServerMessageStr;
            byte[] ServerMessage = Encoding.UTF8.GetBytes(ServerMessageStr);
            SetReport(" Received a message( " + ServerMessageStr + " )from server.");
            ///----capsolating----///
        }


        public void CloseClient(string TimeOutStr)
        {
            IsConnected = false;
            int TimeOut = Convert.ToInt32(TimeOutStr);
            Accept.Close(TimeOut);
            SetReport(" The client is Closed");
            
        }


        public void SetReport(string Info)
        {
            Report += "\r\n" + DateTime.Now + Info;
        }


        public string GetReporter()
        {
            return Report;
        }


    }
}
