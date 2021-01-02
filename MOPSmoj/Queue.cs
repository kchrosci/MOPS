using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Queue
    {
        public List<Packet> packets = new List<Packet>();

        protected double packetWaiting = 0.0;
        protected double packetService = 0.0;
        public int packetNumber = 0;
        protected double surface = 0.0;
        protected double time = 0.0;
        public int packetDiscarded = 0;
        public int delayNumber = 0;
        public double delay = 0;

       
        public void AddPacket(Packet packet)
		{
           // Console.WriteLine($"dodano pakiet: {packet.ArrivalTime}");
            //packetNumber = packets.Count;
            time = packet.ArrivalTime;
            packets.Add(packet);
            // Dorobic zbieranie danych
		}
        public void RemovePacket(double time)
        {
            //pakiet opuszcza kolejke, ale nie wiem ile czasu oczekiwał
            packets.RemoveAt(0);
            // Dorobic zbieranie danych
        }

        internal void ShowResults()
        {
            
        }


	}
}
