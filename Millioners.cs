using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TheMillionersIssues
{
    class Milioner
    {
        private string milionerName;
        private UdpClient sailorPort;

        public Milioner(String name)
        {
            this.milionerName = name;

        }

        public void lookingForSomeTrip()
        {
            this.sailorPort = new UdpClient();
           // Console.Write(this.sailorPort.Client);
            Console.WriteLine("Looking for a new boat...");
            this.sailorPort.Client.Bind(new IPEndPoint(IPAddress.Any, 6565));
            this.sailorPort.BeginReceive(Read_Callback, new object());
            Receive(null);
        }

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BUFFER_SIZE = 1024;
            public byte[] buffer = new byte[BUFFER_SIZE];
            public StringBuilder sb = new StringBuilder();
        }

        public static void Read_Callback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            Socket s = so.workSocket;

            int read = s.EndReceive(ar);

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));
                s.BeginReceive(so.buffer, 0, StateObject.BUFFER_SIZE, 0,
                                         new AsyncCallback(Read_Callback), so);
            }
            else
            {
                if (so.sb.Length > 1)
                {
                    //All of the data has been read, so displays it to the console
                    string strContent;
                    strContent = so.sb.ToString();
                    Console.WriteLine(String.Format("Read {0} byte from socket" +
                                       "data = {1} ", strContent.Length, strContent));
                }
                s.Close();
            }
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes =sailorPort.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("From {0} received: {1} ", ip.Address.ToString(), message);
        }
    }

}
