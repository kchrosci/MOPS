using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Source
    {
        public double timeON = 0; 
        public double timeOFF = 0;
       
      
        Packet packet;
       
        public void TimeGenerator(double beta)
		{
         
            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());
        
		}

        public Packet PacketGeneration(double currentTime)
        {
            packet = new Packet(currentTime);
            return packet;
        }
    }
}
