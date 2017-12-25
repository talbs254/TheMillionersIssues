using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheMillionersIssues
{
    class Program
    {

        public static void Main(string[] args)
        {
            new Thread(funcA).Start();
            new Thread(funcB).Start();

        }
        public static void funcA()
        {
            new Boat("BlackPearl").raiseTheSail();

        }
        public static void funcB()
        {
            new Milioner("JackSparrow").lookingForSomeTrip();
        }

    }

}
