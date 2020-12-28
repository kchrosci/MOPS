using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    public class Event
    {
        public static int ARRIVAL = 1;
        public static int DEPARTURE = 2;

        public double time;
        public int type;

        public Event(double time, int type)
		{
            this.time = time;
            this.type = type;
		}
    }
}
