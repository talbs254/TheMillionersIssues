using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TheMillionersIssues
{
    class Boat
    {
        private static readonly int numOfSailorsPerBoat = 10;

        private static IPAddress IPAdd = initIPAddress();
        private static int currentSocket = 11000;
        private static List<Socket> listenList = new List<Socket>();
        private static List<Socket> acceptList = new List<Socket>();

        private char[] boatName = new char[32];
        private Socket socket;
        private Boolean anchor;

        /// <summary>
        /// find the ip address of the computer
        /// </summary>
        /// <returns> IP object </returns>
        private static IPAddress initIPAddress()
        {
            IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            return ipHostEntry.AddressList[0];
        }

        /// <summary>
        /// initial boat name and socket
        /// </summary>
        /// <param name="name"> name of the boat </param>
        public Boat(String name)
        {
            //initial boat name
            int i;
            for (i = 0; i < name.Length; i += 1)
                this.boatName[i] = name[i];
            while (i < boatName.Length)
            {
                this.boatName[i] = ' ';
                i += 1;
            }

            //open boat socket
            this.socket = new Socket(AddressFamily.InterNetwork,
                               SocketType.Stream, ProtocolType.Tcp);
            this.socket.Bind(new IPEndPoint(IPAdd, currentSocket));
            this.socket.Listen(numOfSailorsPerBoat);
            this.anchor = false;

            //in case there is more than 1 boats
            listenList.Add(socket);

        }

        /// <summary>
        /// broadcast messege every 60 seconds
        /// </summary>
        public void raiseTheSail()
        {
            new System.Threading.Timer((e) =>
            {
                broadcast();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            while (!anchor)
            {
                
            }

           

          

        }
        
        private void broadcast()
        {
            int i, j;
            String boatName = new string(this.boatName);       
            byte[] info = Encoding.ASCII.GetBytes("IntroToNets" + boatName);
            byte[] port = BitConverter.GetBytes(Int16.Parse(((IPEndPoint)socket.LocalEndPoint).Port.ToString()));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(port);
   
            byte[] application = new byte[info.Length + port.Length];
            
            for (i = 0; i < info.Length; i += 1)
                application[i] = info[i];
            for (j = 0; j < port.Length; j += 1, i += 1)
                application[i] = port[j];


            new UdpClient().Send(application, application.Length , new IPEndPoint(IPAddress.Broadcast, 5656));









        }
    }
}