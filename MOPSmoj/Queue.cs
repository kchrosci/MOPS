﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Queue
    {
        public List<Packet> packets = new List<Packet>();

        public int packetNumber = 0;
        public double surface = 0.0;
        protected double time = 0.0;
        public int packetDiscarded = 0;
        public int delayNumber = 0;
        public double delay = 0;
        public double emptyServer = 0;

        public void AddPacket(Packet packet)
		{
            time = packet.ArrivalTime;
            packets.Add(packet);
		}
        public void RemovePacket(double time)
        {
            packets.RemoveAt(0);
        }
	}
}
