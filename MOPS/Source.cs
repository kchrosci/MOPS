using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Source
    {
        public double timeON; 
        public double timeOFF;
       
      
        Packet packet;

        public void ShowTime(double time1, double time2)
		{
            Console.WriteLine($"Time ON is equal to: {time1} ");
            Console.WriteLine($"Time OFF is equal to: {time2} ");
		}

        //public Source(double beta)
        //{
        //    Random random = new Random();
        //    timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
        //    timeOFF = -(beta) * Math.Log(1 - random.NextDouble());
        //}

        public void TimeGenerator(double beta)
		{
         
            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());
        
		}

        public Packet PacketGeneration(double currentTime)
        {
            packet = new Packet(currentTime);
            packet.ShowPacketInfo();
            return packet;
        }
    }
}
