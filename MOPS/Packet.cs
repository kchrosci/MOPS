using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Packet
    {
        public double ArrivalTime { get; set; }
        public double ServiceTime { get; set; }
        public double arrivalTime;
        public double serviceTime = 0.7;


        public Packet(double arrivalTime)
		{
            this.arrivalTime = arrivalTime;
		}
        public void ShowPacketInfo()
		{
            Console.WriteLine(arrivalTime);
		}
    }
}
