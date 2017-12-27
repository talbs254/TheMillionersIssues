using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TheMillionersIssues
{

    class Boat
    {

        private Dictionary<Socket, Tuple<string, int>> millionersDictionary;

        private TcpListener server;
        private String boatName32;
        private String boatName;
        private int port;


     
        public Boat(String name)
        {
            this.millionersDictionary = new Dictionary<Socket, Tuple<String, int>>();
            this.boatName = name;
            this.port = findFreeTcpPort();
            byte[] a = Encoding.ASCII.GetBytes(GetLocalIPAddress());
            server = new TcpListener(IPAddress.Parse(GetLocalIPAddress()), port);
            server.Start();


            //initial boat name
            char[] boatName32 = new char[32];
             int i;
              for (i = 0; i < name.Length; i += 1)
                boatName32[i] = name[i];
              while (i < boatName32.Length)
              {
                boatName32[i] = ' ';
                  i += 1;
              }

            this.boatName32 = new string(boatName32);
              

        }

        private void anchor()
        {
            string exit = Console.ReadLine();
            foreach (Socket client in millionersDictionary.Keys)
                client.Close();
            millionersDictionary.Clear();
            anchor();
        }

        public void raiseTheSail()
        {
            new Thread(anchor).Start();

            new System.Threading.Timer((e) =>
            {
                broadcast();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            /*
            new System.Threading.Timer((e) =>
            {
                List<Socket> listenList = new List<Socket>();
                foreach(Socket socket in millionersDictionary.Keys)               
                    listenList.Add(socket);
                if(listenList.Count>0)
                    Socket.Select(listenList, null, null, 1000);
                foreach(Socket client in millionersDictionary.Keys)
                {
                    if (!listenList.Contains(client))
                    {
                        removeClient(client);
                    }
                    else
                        client.Accept();
                }                            
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            */

            while (true)
            {
                
                TcpClient client = server.AcceptTcpClient();
                millionersDictionary.Add(client.Client, null);
                new Thread(() => clientDialog(client)).Start();                          
             }       
        }

        private void clientDialog(TcpClient client)
        {
            Boolean onBoard = true;
            byte[] buffer = new byte[1024];
            Console.WriteLine("Welcome to The " + boatName + "! What is your name?");
            client.Client.Receive(buffer);
            string newMillioner = getStringFromBuffer(buffer);
            millionersDictionary[client.Client] = new Tuple<string, int>(newMillioner, 0);
            string msg = "A Millionaire named " + newMillioner + " has joined the boat. The richest person on the boat right now is " + findRichestMillioner();
            Console.WriteLine(msg);
            sendMessageToAllClients(msg);
            try
            {
                while (onBoard)
                {
                    buffer = new byte[1024];
                    client.Client.Receive(buffer);
                    string newMsg = getStringFromBuffer(buffer);
                    string richestMill = null;
                    int tryParse;
                    if (newMsg.Equals("\r\n"))
                    {
                        onBoard = false;
                        removeClient(client.Client);

                    }
                    else if (int.TryParse(newMsg, out tryParse))
                    {
                        millionersDictionary[client.Client] = new Tuple<string, int>(millionersDictionary[client.Client].Item1, Int32.Parse(newMsg));
                        richestMill = findRichestMillioner();
                        newMsg = millionersDictionary[client.Client].Item1 + " has updated his/her income. The richest person on the boat right now is " + richestMill;
                        sendMessageToAllClients(newMsg);
                        Console.WriteLine(newMsg);
                    }
                }
            }catch(SocketException)
            {
                removeClient(client.Client);
            }
                 
        }

        private void removeClient(Socket client)
        {
            string leaver = millionersDictionary[client].Item1;
            millionersDictionary.Remove(client);
            client.Close();
            string richestMill = findRichestMillioner();
            string newMsg = leaver + " has left the boat. the richest person on boat right now is " + richestMill;
            sendMessageToAllClients(newMsg);
            Console.WriteLine(newMsg);
        }
        private void sendMessageToAllClients(string msg)
        {
            foreach (Socket millioner in millionersDictionary.Keys)
                millioner.Send(Encoding.ASCII.GetBytes(msg));

        }

        private String getStringFromBuffer(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length && buffer[i] != 0; i++)
                sb.Append((char)buffer[i]);
            return sb.ToString();

        }




        private void broadcast()
        {

            byte[] convertedPort = BitConverter.GetBytes(Int16.Parse(port.ToString()));
             if (BitConverter.IsLittleEndian)
                 Array.Reverse(convertedPort);
            String applicationMessege = "IntroToNets" + boatName32;
            Byte[] info = Encoding.ASCII.GetBytes(applicationMessege);


            
            byte[] application = new byte[info.Length + convertedPort.Length];
            int i, j;
            for (i = 0; i < info.Length; i += 1)
                application[i] = info[i];
            for (j = 0; j < convertedPort.Length; j += 1, i += 1)
                application[i] = convertedPort[j];

    
            new UdpClient().Send(application, application.Length , new IPEndPoint(IPAddress.Broadcast, 1515));

        }
        /// <summary>
        /// find available port in system
        /// </summary>
        /// <returns> available port </returns>
        private static int findFreeTcpPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        
        private string findRichestMillioner()
        {
            int maxSum = 0;
            string richestMillioner=null;
            foreach(Socket socket in millionersDictionary.Keys)
            {
                if (maxSum <= millionersDictionary[socket].Item2)
                {
                    richestMillioner = millionersDictionary[socket].Item1;
                    maxSum = millionersDictionary[socket].Item2;
                }
            }
            return richestMillioner;
        }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


    }

    
}