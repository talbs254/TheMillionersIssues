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

        private List<Socket> millionerstList;
        private Dictionary<String, Tuple<int, Socket>> millionersDictionary;
        private TcpListener server;
        private String boatName;
        private int port;
        private Boolean anchor;


     
        public Boat(String name)
        {
         
            this.millionersDictionary = new Dictionary<string, Tuple<int, Socket>>();
            this.millionerstList = new List<Socket>();
            this.anchor = false;
            this.boatName = name;
            this.port = findFreeTcpPort();
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            //initial boat name
            char[] boatNameArray = new char[32];
             int i;
              for (i = 0; i < name.Length; i += 1)
                boatNameArray[i] = name[i];
              while (i < boatName.Length)
              {
                boatNameArray[i] = ' ';
                  i += 1;
              }

            this.boatName = new string(boatNameArray);
              

        }

        public void raiseTheSail()
        {
            new System.Threading.Timer((e) =>
            {
                broadcast();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));


            while (!anchor)
            {
                /*
                Socket socket = server.AcceptSocket();
                if (socket.Connected)
                {
                    millionerstList.Add(socket);
                    millionerJoindToBoat();
                }
                */
             }
         

        }
        
        public void broadcast()
        {
                    
           // String convertedPort = Convert.ToString(port, 2);

            byte[] convertedPort = BitConverter.GetBytes(Int16.Parse(port.ToString()));
             if (BitConverter.IsLittleEndian)
                 Array.Reverse(convertedPort);
            String applicationMessege = "IntroToNets" + boatName;
            Byte[] info = Encoding.ASCII.GetBytes(applicationMessege);


            
            byte[] application = new byte[info.Length + convertedPort.Length];
            int i, j;
            for (i = 0; i < info.Length; i += 1)
                application[i] = info[i];
            for (j = 0; j < convertedPort.Length; j += 1, i += 1)
                application[i] = convertedPort[j];

    
            new UdpClient().Send(application, application.Length , new IPEndPoint(IPAddress.Broadcast, 5656));

        }
        /// <summary>
        /// find available port in system
        /// </summary>
        /// <returns> available port </returns>
        private static int findFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private void millionerJoindToBoat()
        {
            foreach(Socket socket in millionerstList)
            {
                String messege = "Welcome to " + boatName + "! What is your name?";
                socket.Send(Encoding.ASCII.GetBytes(messege));
            }
        }

        
    }
}