using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Source
    {
        public double timeON; 
        public double timeOFF;
        public bool state;
        Packet packet;


        public Source(double beta)
        {
            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());
        }

        public double PacketGeneration(double currentTime, double packetBreak, double beta)
        {
            state = true;
            double currentTimeON = 0;

            while (state)
            {              
                if (currentTimeON > timeON)
				{
                    state = false;
                    
                }                 
				else
				{
                    packet = new Packet(currentTime);
                    packet.ShowPacketInfo();
                    currentTimeON += packetBreak;
                    currentTime += packetBreak;
                }              
            }
            currentTime += timeOFF;

            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());

            return currentTime;
        }
    }
}
