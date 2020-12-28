using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Source
    {
        public double timeON; 
        public double timeOFF;
        public bool state = false;
        Packet packet;

        public Source(double beta)
        {
            Random random = new Random();
            timeON = -(beta) * Math.Log(1 - random.NextDouble()); //beta - sredni czas trwania stanu ON/OFF
            timeOFF = -(beta) * Math.Log(1 - random.NextDouble());
            Console.WriteLine("xd");
        }

        public void PacketGeneration(double currentTime)
        {
            state = true;

            while(state)
            {
                if (currentTime > timeON)
                    state = false;
                else
                    packet = new Packet(currentTime);
            }
        }
    }
}
