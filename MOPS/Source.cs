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

        public double PacketGeneration(double currentTime, double packetBreak)
        {
            state = true;
            
            while (state)
            {
                
                if (currentTime > timeON)
				{
                    state = false;
                    currentTime = 0;

                }
                   
				else
				{
                    packet = new Packet(currentTime);
                    packet.ShowPacketInfo();
                    currentTime += packetBreak;
                }
                
            }
            return currentTime;
        }
    }
}
