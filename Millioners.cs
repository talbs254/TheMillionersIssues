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
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 5656);
            this.sailorPort = new UdpClient(RemoteIpEndPoint);
            Console.WriteLine("Looking for a new boat...");
         
            // Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes = this.sailorPort.Receive(ref RemoteIpEndPoint);
            Byte[] portBytes = { receiveBytes[receiveBytes.Length - 1], receiveBytes[receiveBytes.Length - 2] };

            
            string applicationMsg = Encoding.ASCII.GetString(receiveBytes);      
            int returnPort = BitConverter.ToInt16(portBytes, 0);

            Console.WriteLine("Requesting to board The " + applicationMsg.Substring("IntroToNets".Length, 32/*bytes*/));

            
          
       
        }
    }

}
