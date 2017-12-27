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
           
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 5656);
            sailorPort = new UdpClient(ipEndPoint);
            Console.WriteLine("Looking for a new boat...");
            Byte[] receiveBytes = sailorPort.Receive(ref ipEndPoint);
            Byte[] portBytes = { receiveBytes[receiveBytes.Length - 1], receiveBytes[receiveBytes.Length - 2] };           
            string applicationMsg = Encoding.ASCII.GetString(receiveBytes);      
            int returnPort = BitConverter.ToInt16(portBytes, 0);

            Console.WriteLine("Requesting to board The " + applicationMsg.Substring("IntroToNets".Length, 32/*bytes*/));
            TcpClient tcpConnection = new TcpClient("server", returnPort); // add port

            if (tcpConnection.Connected)
            {

            }


            
          
       
        }
    }

}
