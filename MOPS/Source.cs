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

        public void ShowTime(double time1, double time2)
		{
            Console.WriteLine($"Time ON is equal to: {time1} ");
            Console.WriteLine($"Time OFF is equal to: {time2} ");
		}

        public Source(double beta)
        {
            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());
        }

        public double PacketGeneration(double currentTime, double packetBreak, double beta,double simulationTime)
        {
            Console.WriteLine("CZAS SYSTEMU " + currentTime);
            ShowTime(timeON, timeOFF);
            

            state = true;
            double currentTimeON = 0;

            while (state && simulationTime > currentTime)
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
            currentTime = currentTime + timeOFF - (currentTimeON - timeON);

            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());

            return currentTime;
        }
    }
}
