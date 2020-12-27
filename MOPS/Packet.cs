using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Packet
    {
        protected double ArrivalTime { get; set; }
        protected double ServiceTime { get; set; }
        protected double arrivalTime;
        protected double serviceTime;
        public Packet()
		{
            arrivalTime = ArrivalTime;
            serviceTime = ServiceTime;
		}
    }
}
