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

        /// <summary>
        /// millioner looking for a boat to join using UDP Connection
        /// </summary>
        public void lookingForSomeTrip()
        {
            try
            {
                Console.WriteLine("Looking for a new boat...");
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1515);
                sailorPort = new UdpClient(ipEndPoint);
                Byte[] receiveBytes = sailorPort.Receive(ref ipEndPoint);
                Byte[] portBytes = { receiveBytes[receiveBytes.Length - 1], receiveBytes[receiveBytes.Length - 2] };
                string applicationMsg = Encoding.ASCII.GetString(receiveBytes);
                int returnPort = BitConverter.ToInt16(portBytes, 0);
                String boatName = applicationMsg.Substring("IntroToNets".Length, 32/*bytes*/);
                Console.WriteLine("Requesting to board The " + boatName);

                TcpClient tcpConnection = new TcpClient();
                tcpConnection.Connect(new IPEndPoint(ipEndPoint.Address, returnPort));

                while (tcpConnection.Connected)
                {
                    Console.WriteLine("I am now aboard " + boatName);
                    try
                    {
                        //send name to boat
                        tcpConnection.Client.Send(Encoding.ASCII.GetBytes(milionerName));

                        //enable user to enter income or ask to leave the boat
                        userEnterdInput(tcpConnection, boatName);

                        //while connection is connected
                        while (tcpConnection.Connected)
                        {
                            byte[] buffer = new byte[1024];
                            tcpConnection.Client.Receive(buffer);
                            Console.WriteLine(getStringFromBuffer(buffer));
                        }


                    }
                    catch (Exception e)
                    {
                        lookingForSomeTrip();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not open TCP connection");
                lookingForSomeTrip();
            }

        }

        private String getStringFromBuffer(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length && buffer[i] != 0; i++)
                sb.Append((char)buffer[i]);
            return sb.ToString();

        }
        /// <summary>
        /// open new thread to check if the user entered input
        /// </summary>
        /// <param name="tcpConnection"></param>
        private void userEnterdInput(TcpClient tcpConnection, String boatName)
        {
            new System.Threading.Timer((e) =>
            {
                string input = Console.ReadLine();

                //user entered ENTER
                if (input.Length == 1 && input[0] == '\n')
                    tcpConnection.Close();
                else
                {
                    //user entered income
                    int income;
                    if (int.TryParse(input, out income))
                        tcpConnection.Client.Send(Encoding.ASCII.GetBytes(input));
                    else
                        Console.WriteLine("Invalid input. You can tell me your income or press ENTER if you want to leave " + boatName);
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
    }

}
