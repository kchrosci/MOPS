using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Queue
    {
        public double time = 0.0;
        public double simulationTime = 100.0;
        public double packetBreak = 0.5;
        public bool isEmpty = true;
        Source source;
        List<Event> events;

        public Queue()
        {
            source = new Source(5);
        }

        public void Checktime()
        {
            while(true)
            {
                if (simulationTime >= time)
                    break;
                else
                {
                    
                }
            }
        }


    }
}
