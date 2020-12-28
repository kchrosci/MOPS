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

        public bool sourceState;
        Event eventObj;
        Source source;

        public List<Event> events;
        public double beta = 5;

        public Queue()
        {
            source = new Source();
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
                    source.TimeGenerator(beta);
                    Console.WriteLine("CZAS SYSTEMU " + time);
                    ShowTime(source.timeON, source.timeOFF);
                    sourceState = true;
                    double currentTimeON = 0;

                    while (sourceState && simulationTime > time)
                    {
                       
                        if (currentTimeON > source.timeON)
                        {
                            sourceState = false;
                        }
                        else
                        {
                            eventObj = new Event(time);

                            source.PacketGeneration(time);
                            events.Add(eventObj);

                            currentTimeON += packetBreak;
                            time += packetBreak;
                        }
                    }
                    time = time + source.timeOFF - (currentTimeON - source.timeON);
                }
            }


        foreach(var e in events)
			{
                Console.WriteLine(e.time);
			}
        }
        public void ShowTime(double time1, double time2)
        {
            Console.WriteLine($"Time ON is equal to: {time1} ");
            Console.WriteLine($"Time OFF is equal to: {time2} ");
        }


    }
}
