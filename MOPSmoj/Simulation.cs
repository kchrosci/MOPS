using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOPS
{
	class Simulation
	{
		#region Properties

		public double PacketBreak { get; set; }		= 0.2;
		public double ServiceTime { get; set; }		= 0.08;
		public double BetaON { get; set; }			= 10;
		public double BetaOFF { get; set; }			= 10;
		public double Time { get; set; } 
		public double SimulationTime { get; set; }	= 20000;
		public int QueueLength { get; set; }		= 5;
		public int NumberOfSources { get; set; }	= 4;

		public List<Event> events = new List<Event>();
		public List<Source> sources = new List<Source>();
		Queue queue;
		Event _event;
		Source source;

		public int sourceIndex;
		#endregion

		internal void StartSimulation()
		{	
			for (int i = 0; i < NumberOfSources; i++)
            {
				source = new Source();
				sources.Add(source);
			}
			
			queue = new Queue();

			Time = 0;
			
			while (Time < SimulationTime)
			{
				for (int n = 0; n < NumberOfSources; n++) //wyslanie pierwszego pakietu przez wiele zrodel
				{					
					if (Time == sources[n].CurrentSourceTime + sources[n].LastSourceTime) //zabezpieczenie zeby nie wysylalo z kazdego zrodla w jednym momencie tylko asynchronicznie
                    {
						sources[n].LastTimeOFF += sources[n].TimeOFF;
						sources[n].LastTimeON += sources[n].TimeON;

						sources[n].TimeGenerator(BetaON,BetaOFF, n);

						sources[n].CurrentSourceTime = sources[n].TimeOFF + sources[n].TimeON;
						sources[n].LastSourceTime = sources[n].LastTimeOFF + sources[n].LastTimeON;

						_event = new Event(Time, Event.Arrival, n);

						AddEventToList(_event);
					}					
				}

				int i = 0;

				while (true) //ogolnie plan jest taki ze sprawdza zrodlo i jak jest events[0] z tego zrodla to leci caly kod a jak z innego to brake i sprawdza kolejne zrodlo
				{
					while (sources[i].CurrentSourceTime > Time - sources[i].LastSourceTime && events.Count != 0)
					{
						if (Time > SimulationTime) break;

						SortEventList();

						if (Time != events[0].Time)
							break;

						if (CheckFirstEventSource() != i)
                        {
							i++;

							if (i == NumberOfSources) i = 0;

							continue;
                        }

						if (events.Any() && events[0].Time <= SimulationTime)
						{
							_event = TakeNextEvent();
						}

						Console.WriteLine($"\n[{i}] Current time of the server is: " + Time);

						#region Main if
						if (_event != null && _event.Type == 1 && Time < SimulationTime)
						{
							queue.packetNumber++;
							Console.WriteLine("Number of packet: " + queue.packetNumber);

							if (Time + PacketBreak <= sources[i].TimeON + sources[i].LastSourceTime && Time + PacketBreak <= SimulationTime)
							{
								AddEventToList(new Event(Time + PacketBreak, Event.Arrival, i));
								Console.WriteLine($"The system ARRIVAL event is scheduled on: {Time + PacketBreak}");
							}

							if (QueueLength >= queue.packets.Count)
							{
								queue.AddPacket(new Packet(Time, ServiceTime, i));
							}
							else
							{
								queue.packetDiscarded++;
								Console.WriteLine("Packet DISCARDED! NUMBER: " + queue.packetDiscarded);
							}

							//Planowanie pierwszego opuszczenia systemu po generacji dla pakietu ktory został wzięty do obsługi.
							//Cała reszta zostanie zaplanowana w trakcie usuwania kolejnych pakietow z listy
							if (queue.packets.Count > 1) { }
							else
							{
								AddEventToList(new Event(Time + ServiceTime, Event.Departure, i));
								Console.WriteLine($"The system EXIT event was scheduled: {Time + ServiceTime}");
							}

							if (CheckSourcesOrEventTime())
							{
								if (queue.packets.Count > 1)
								{
									queue.surface += (queue.packets.Count - 1) * (sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime - Time);
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("QUEUE: " + (queue.packets.Count - 1));
								}
								Time = FindShortestTime();
							}
							else
							{
								if (queue.packets.Count > 1)
								{
									queue.surface += (queue.packets.Count - 1) * (events[0].Time - Time);
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("QUEUE: " + (queue.packets.Count - 1));
								}

								Time = events[0].Time;
							}

							_event = null;

							if (Time >= SimulationTime) Time = SimulationTime;
							
							i++;
							
							if (i == NumberOfSources) i = 0;
						}
						#endregion

						#region Main else if
						else if (_event != null && _event.Type == 2 && Time <= SimulationTime)
						{
							queue.delay += (Time - queue.packets[0].ArrivalTime - ServiceTime);
							queue.delayNumber++;
							Console.WriteLine("The current DELAY is: " + queue.delay);

							Console.WriteLine($"Removed packet at: {Time}. Arrival time: {queue.packets[0].ArrivalTime}");
							queue.RemovePacket(Time);
							if (queue.packets.Count <= 0)
							{
								if (events.Any())
                                {
									queue.emptyServer += Math.Min(Math.Min(sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime, SimulationTime), events[0].Time) - Time;
									Time = Math.Min(events[0].Time, sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime);
								}						
								else
								{
									queue.emptyServer += Math.Min(sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime, SimulationTime) - Time;
									queue.surface += (queue.packets.Count * (sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime - Time));
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("QUEUE: " + queue.packets.Count);
									Time = sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime;
								}
							}
							else
							{
								AddEventToList(new Event(Time + ServiceTime, Event.Departure, queue.packets[0].SourceNumber));
								Console.WriteLine($"The system EXIT event was scheduled on: {Time + ServiceTime}");

								if (CheckSourcesOrEventTime())
								{
									queue.surface += (queue.packets.Count - 1) * (sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime - Time);
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("QUEUE: " + (queue.packets.Count - 1));
									Time = sources[FindIndex()].CurrentSourceTime + sources[FindIndex()].LastSourceTime;
								}
								else
								{
									if (!(events[0].Time > SimulationTime))
									{
										queue.surface += (queue.packets.Count - 1) * (events[0].Time - Time);
										Console.WriteLine("SURFACE: " + queue.surface);
										Console.WriteLine("QUEUE: " + (queue.packets.Count - 1));
									}
									Time = events[0].Time;
								}
							}

							_event = null;

							if (Time >= SimulationTime)	Time = SimulationTime;

							i++;

							if (i == NumberOfSources) i = 0;
						}
						#endregion

						#region Main else
						else 
						{
							Console.WriteLine($"ERROR");
						}
						#endregion
					}
					break;
				}
			}
			Console.WriteLine("\nDiscarded after time: " + queue.packets.Count);
			queue.packetDiscarded += queue.packets.Count;
			Console.WriteLine($"Simulation time has finished. Total time: {Time}");
		}

		private Event TakeNextEvent()
		{
			Event e = events.ElementAt(0);
			events.RemoveAt(0);
			return e;				
		}

		private void AddEventToList(Event _event)
		{
			int i = 0;
			foreach(var e in events)
			{
				if(e.Time > _event.Time)
				{
					break;
				}
				else
				{
					i++;
				}
			}
			events.Insert(i, _event);			
		}

		private int CheckFirstEventSource()
        {
			if (events.Count != 0)
				return events[0].SourceNumber;
			else
				return 10;
		}

		public void SortEventList()
		{
			for (int i = 0; i < events.Count - 1; i++)
			{
				while (true)
				{
					if (events[i].Time > events[i + 1].Time)
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

		public bool CheckSourcesOrEventTime()
        {
			foreach (Source s in sources)
            {
				if (s.LastSourceTime + s.CurrentSourceTime < events[0].Time)
				{
					return true;
				}
            }
			return false;
        }
		
		public double FindShortestTime()
        {
			double min = double.MaxValue;

			foreach (Source s in sources)
			{
				min = Math.Min(min, s.CurrentSourceTime + s.LastSourceTime);
			}
			return min;
		}

		public int FindIndex()
		{
			double min = double.MaxValue;
			double[] table = new double[NumberOfSources];
			int i = 0;
			foreach (Source s in sources)
			{
				double x = s.LastSourceTime + s.CurrentSourceTime;
				
				table[i] = x;

				if (table[i] < min)
				{
					min = table[i];
					sourceIndex = i;
				}
				i++;
			}
			return sourceIndex;
		}

		internal void ShowResults()
		{
			Console.WriteLine();
			Console.WriteLine("************************ RESULTS ************************");
			Console.WriteLine($"Total time: {Time}" );
			Console.WriteLine($"Number of packets sent: {queue.packetNumber}");
			Console.WriteLine($"Mean number of packets in queue: {queue.surface/Time}" );
			Console.WriteLine($"Mean packet delay: {Math.Round((queue.delay/queue.delayNumber), 15)}" );
			Console.WriteLine($"Mean server load: {(Time - queue.emptyServer)/Time}" );
			Console.WriteLine($"Packet loss level: {(double)queue.packetDiscarded / queue.packetNumber * 100}%");
		}
	}
}
