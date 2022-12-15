using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TVS
{
    class Packets
    {


        string sample;
        string Source_ID;
        string Target_ID;
        string TYPE;



        public T DeEncapsulation<T>(byte[] DATA)
        {
            T Result;


            if (typeof(T) == typeof(DataManagment))
            {
                DataManagment Temp = new DataManagment();
                Temp.Data = DATA;

                Result = (T)Convert.ChangeType(Temp, typeof(T));
                return Result;
            }


            if (typeof(T) == typeof(Header))
            {
                Header Temp = new Header();
                string data = Encoding.UTF8.GetString(DATA);
                string[] Datakeeper = data.Split('$');
                Temp.Size = Convert.ToInt64(Datakeeper[0]);
                Temp.SourceID = Datakeeper[1];
                Temp.TargetID = Datakeeper[2];
                Datakeeper[3] = Datakeeper[3][0].ToString();
                Temp.Type = (packettype)Convert.ToInt32(Datakeeper[3]);
                Result = (T)Convert.ChangeType(Temp, typeof(T));
                return Result;

            }


            Result = (T)Convert.ChangeType(0, typeof(T));
            return Result;
        }


        public T StringDeEncapulation<T>(byte[] DATA)
        {
            T Result;


            if (typeof(T) == typeof(LoginDataManagment))
            {
                LoginDataManagment Temp = new LoginDataManagment();
                string data = Encoding.UTF8.GetString(DATA);
                string[] Datakeeper = data.Split('%');

                Temp.Header = Datakeeper[0];
                Temp.Data = Datakeeper[1];

                Result = (T)Convert.ChangeType(Temp, typeof(T));
                return Result;
            }


            if (typeof(T) == typeof(Login))
            {
                Login Temp = new Login();
                string data = Encoding.UTF8.GetString(DATA);
                string[] Datakeeper = data.Split('^');
                Temp.Username = Datakeeper[0];
                Temp.Password = Datakeeper[1];
                Result = (T)Convert.ChangeType(Temp, typeof(T));
                return Result;
            }


            if (typeof(T) == typeof(Header))
            {
                Header Temp = new Header();
                string data = Encoding.UTF8.GetString(DATA);
                string[] Datakeeper = data.Split('$');
                Temp.Size = Convert.ToInt64(Datakeeper[0]);
                Temp.SourceID = Datakeeper[1];
                Temp.TargetID = Datakeeper[2];
                Temp.Type = (packettype)Convert.ToInt32(Datakeeper[3]); ;
                Result = (T)Convert.ChangeType(Temp, typeof(T));
                return Result;

            }


            Result = (T)Convert.ChangeType(0, typeof(T));
            return Result;
        }


        public string ReturnToString<T>(T Data)
        {
            string result;
            FormatterConverter Format = new FormatterConverter();


            if (Data is Login)
            {
                Login Temp = (Login)Format.Convert(Data, typeof(Login));
                result = Temp.Username + "^" + Temp.Password;

                return result;
            }


            if (Data is Header)
            {
                Header Temp = (Header)Format.Convert(Data, typeof(Header));
                sample = (Temp.Size).ToString(); // 1 means OK  0 means Not Ok
                Source_ID = Temp.SourceID;
                Target_ID = Temp.TargetID;
                TYPE = ((short)Temp.Type).ToString();

                result = sample + "$" + Temp.SourceID + "$" + Temp.TargetID + "$" + TYPE;
                return result;

            }


            if (typeof(T) == typeof(LoginDataManagment))
            {
                LoginDataManagment Temp = (LoginDataManagment)Format.Convert(Data, typeof(LoginDataManagment));



                result = Temp.Header + "%" + Temp.Data;
                return result;
            }


            return "";
        }


        public struct DataManagment
        {
            public byte[] Data;
            public byte[] Header;

        }


        public struct Header
        {
            public string TargetID;
            public string SourceID;
            public packettype Type;
            public long Size;
        }


        public enum packettype
        {
            Login,
            NormalData,
            Voice,
            SignUp,
            Message,

        }


        public struct LoginDataManagment
        {
            public string Data;
            public string Header;

        }


        public struct Login
        {
            public string Username;
            public string Password;
        }




        public struct NormalData
        {
            public string Data;

        }


        public struct voice
        {
            public byte[] VOICE;
        }


    }
}


