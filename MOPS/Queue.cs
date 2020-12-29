﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Queue
    {
        public double time = 0.0;
        public double generationTime = 0.0;
        public double outTime = 0;
        public double simulationTime = 5.0;
        public double packetBreak = 0.5;
        public int queueLength = 0;
        public int queueSize = 10;
        
        public bool isServerEmpty = true;

        public bool sourceState = true;
        Event eventObj;
        Source source;
        Packet packet;
        Packet nextPacket;

        public List<Event> events;
        public List<Packet> packetQueue;
        public double beta = 1;
        public double delay = 0;
        public double delayAmount = 0;
        public int packetLoss = 0;

        double lastTimeON = 0;
        double lastTimeOFF = 0;

        public Queue()
        {
            source = new Source();
        }

        public void StartSimulation()
        {
            events = new List<Event>();
            packetQueue = new List<Packet>();
           
            while (true)
            {
               
                if (time >= simulationTime)
                    break;
                else
                {
                    lastTimeON += source.timeON;
                    lastTimeOFF += source.timeOFF;
                    nextPacket = null;
                    source.TimeGenerator(beta);
                    double currentTimeON = 0;
                    ShowTime(source.timeON, source.timeOFF);
                    //sourceState = true;
                    
                    while (source.timeON + source.timeOFF > (time - lastTimeOFF - lastTimeON))
                    {

                        sortEventList();

                        if (events.Count != 0 && events[0].time > time && (time != lastTimeOFF + lastTimeON || time != 0))
                        {
                            //if ()
                            
                           time = events[0].time;
                           
                            //events.RemoveAt(0);
                            // czy tu usuwac z listy czy nie
                        }

                        currentTimeON = time - lastTimeON - lastTimeOFF;
                        Console.WriteLine("CZAS SYSTEMU " + time);

                        if (sourceState && simulationTime > time && ((events.Count == 0 || events[0].type == 1) || (events[0].time>time)))
                        {

                            
                            if (sourceState)
                            {
                                if (nextPacket == null)
                                    packet = source.PacketGeneration(time);
                                else
                                    packet = nextPacket;
                                //eventObj = new Event(time, 1);
                                //events.Add(eventObj);

                                if (currentTimeON + packetBreak < source.timeON)
                                { 
        //Planowanie pakietu czyli jego juz generwoanie ale z czasem takim jak ponizej czyli na poczatku 0 +0.5.
                                    nextPacket = source.PacketGeneration(generationTime + packetBreak);
                                    eventObj = new Event(generationTime + packetBreak, 1);
                                    events.Add(eventObj);
                                }

                                if (isServerEmpty)
                                {
                                    delay += 0;
                                    delayAmount += 1;
                                    isServerEmpty = false;
                                    eventObj = new Event(outTime + packet.serviceTime, 2);                                    
                                    events.Add(eventObj);
                                    

                                    outTime += packet.serviceTime;
                                }
                                else
                                {
                                    eventObj = new Event(outTime + packet.serviceTime, 2);
                                    events.Add(eventObj);

                                    packetQueue.Add(packet);
                                    queueLength += 1;
                                    
                                    //Spradzamy czy kolejka jest wieksza niż jej wielkość
                                    if (queueLength >= queueSize)
                                        packetLoss += 1;
                                }

                                
                            }

                            if (currentTimeON + packetBreak > source.timeON)
                            {
                                sourceState = false;
                            }

                            //events.RemoveAt(0);
                            generationTime += packetBreak;
                        }
                        else if (simulationTime > time && events.Count != 0 && events[0].type == 2 && events[0].time <= time)
                        {
                            if (queueLength == 0)
                            {
                                isServerEmpty = true;
                                //events.RemoveAt(0);
                            }
                            else
                            {
                                queueLength -= 1;
                                delay += (time - packetQueue[0].arrivalTime);
                                delayAmount += 1;
                                //eventObj = new Event(outTime + packet.serviceTime, 2);
                                //events.Add(eventObj);
                                packetQueue.RemoveAt(0);
                            }

                            //events.RemoveAt(0);
                            outTime += packet.serviceTime;
                        }
                        else
                        {
                            //time += lastTimeON + lastTimeOFF;
                            time = source.timeOFF + source.timeON + lastTimeON + lastTimeOFF;
                            generationTime = time;
                            outTime = time;
                            sourceState = true;
                            break;
                        }

                        if (events.Count != 0 && time >= events[0].time)
                        {
                            Console.WriteLine("Usunieto event " + events[0].time +" typ: " + events[0].type);
                            events.RemoveAt(0);

                        }

      //                  if(events.Count!=0  && source.timeON + source.timeOFF < events[0].time )
						//{
      //                      break;
						//}
                    }
                }
            }
        }
        public void ShowTime(double time1, double time2)
        {
            Console.WriteLine($"Time ON is equal to: {time1} ");
            Console.WriteLine($"Time OFF is equal to: {time2} ");
        }

        public void sortEventList()
        {
            for (int i = 0; i < events.Count - 1; i++)
            {
                while(true)
                {
                    if (events[i].time > events[i + 1].time)
                    {
                        Event e = events[i];
                        events.RemoveAt(i);
                        events.Add(e);
                    }
                    else
                        break;
                }              
            }
        }
    }
}
