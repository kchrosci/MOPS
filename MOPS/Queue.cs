using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Queue
    {
        public double time = 0.0;
        public double simulationTime = 50.0;
        public double packetBreak = 0.5;
        public bool isEmpty = true;
        Source source;
        public List<Event> events;
        public double beta = 5;

        public Queue()
        {
            source = new Source(beta);
        }

        public void StartSimulation()
        {
            events = new List<Event>();
            while(true)
            {
                if (time >= simulationTime)
                    break;
                else
                {                 
                    time = source.PacketGeneration(time, packetBreak, beta , simulationTime);















                }
            }
        }


    }
}
