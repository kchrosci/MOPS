﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    public class Event
    {
        public static int Arrival { get; } = 1;
        public static int Departure { get; } = 2;

        public double Time { get; }
        public int Type { get; }
        public int SourceNumber { get; }

        public Event(double time, int type, int sourceNumber)
		{
            Time = time;
            Type = type;
            SourceNumber = sourceNumber;
		}
    }
}
