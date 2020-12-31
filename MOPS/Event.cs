using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    public class Event
    {
        public double time;
        public int type;

        public Event(double time, int type)
		{
            this.time = time;
            this.type = type;
		}
    }
}
