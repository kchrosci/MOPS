using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Packet
    {
        public double ArrivalTime { get; }
        public double ServiceTime { get; }


        public Packet(double arrivalTime, double serviceTime)
		{
            ArrivalTime = arrivalTime;
            ServiceTime = serviceTime;
		}

        public void ShowPacketInfo()
		{
            Console.WriteLine(ArrivalTime);
		}
    }
}
