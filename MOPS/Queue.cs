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
        public int queueLength = 0;
        public int queueSize = 10;
        
        public bool isServerEmpty = true;

        public bool sourceState;
        Event eventObj;
        Source source;
        Packet packet;

        public List<Event> events;
        public double beta = 5;
        public double packetDelay = 0;
        public int packetLoss = 0;

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
                            eventObj = new Event(time, 1);
                            packet = source.PacketGeneration(time);
                            events.Add(eventObj);

                            //algorytm zdarzenia przybycia pkaietu
                            if (isServerEmpty)
                            {
                                packet.delay = 0;
                                packetDelay += 1;
                                isServerEmpty = false;
                                eventObj.time = time + packet.serviceTime;
                                eventObj.type = 2;
                                events.Add(eventObj);

                                for (int i = 0; i < events.Count; i++)
                                {
                                    if (events[i].type == 1)
                                    {
                                        events.RemoveAt(i);
                                        break;
                                    }                                        
                                }                                   
                            }
                            else
                            {
                                queueLength += 1;

                                if (queueLength >= queueSize)
                                    packetLoss += 1;
                            }

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
