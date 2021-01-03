using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Source
    {
        public double TimeON { get; set; } = 0; 
        public double TimeOFF { get; set; } = 0;
        public double LastTimeOFF { get; set; } = 0;
        public double LastTimeON { get; set; } = 0;
        public double CurrentSourceTime { get; set; } = 0;
        public double LastSourceTime { get; set; } = 0;

        Packet packet;

        public void TimeGenerator(double beta, int number)
        {

            Random random = new Random();
            TimeON = -(beta) * Math.Log(1 - random.NextDouble());
            TimeOFF = -(beta) * Math.Log(1 - random.NextDouble());
            Console.WriteLine($"\nSOURCE: {number} * ***** TIME OF STATES ON/OFF HAS BEEN GENERATED *******\nTime ON: {TimeON}\nTime OFF: {TimeOFF}");
        
		}

        /*public Packet PacketGeneration(double currentTime, double servicetime)
        {
            packet = new Packet(currentTime,servicetime);
            return packet;
        }*/
    }
}
