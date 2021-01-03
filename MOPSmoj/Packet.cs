using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Packet
    {
        public double ArrivalTime { get; }
        public double ServiceTime { get; }
        public int SourceNumber { get; }

        public Packet(double arrivalTime, double serviceTime, int sourceNumber)
		{
            ArrivalTime = arrivalTime;
            ServiceTime = serviceTime;
            SourceNumber = sourceNumber;
		}
    }
}
