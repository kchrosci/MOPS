using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOPS
{
	class Simulation
	{
		#region Properties

		//public double LastTimeOFF { get; set; } = 0;
		//public double LastTimeON { get; set; } = 0;
		public double PacketBreak { get; set; } = 0.2;
		public double ServiceTime { get; set; } = 0.5;
		public double Beta { get; set; } = 1;
		//public double TimeON { get; set; }
		//public double TimeOFF { get; set; }
		public double Time { get; set; }
		public double SimulationTime { get; set; } = 5;
		public int QueueLength { get; set; } = 5;
		//public double CurrentSourceTime { get; set; }
		//public double LastSourceTime { get; set; }
		public int NumberOfSources { get; set; } = 2;
		public double TimeDifference { get; set; } = 0.03;


		public List<Event> events = new List<Event>();

		Source source;
		public List<Source> sources = new List<Source>();

		Queue queue;
		Event _event;
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
				//int n = 0;

				for (int n = 0; n < NumberOfSources; n++) //wyslanie pierwszego pakietu przez wiele zrodel
				{					
					if (Time == sources[n].CurrentSourceTime + sources[n].LastSourceTime) //zabezpieczenie zeby nie wysylalo z kazdego zrodla w jednym momencie tylko asynchronicznie
                    {
						sources[n].LastTimeOFF += sources[n].TimeOFF;
						sources[n].LastTimeON += sources[n].TimeON;

						sources[n].TimeGenerator(Beta, n);

						/*n++;

						if (n > 0)
                        {
							sources[n].TimeON += TimeDifference * n;
							sources[n].TimeOFF += TimeDifference * n;
                        }*/
						//proba asynchronizacji zrodel ale jest do cipy

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

						if (events.Count == 0 && queue.packets.Count == 0)
						{
							for (int j = 0; j < NumberOfSources - 1; j++)
                            {
								Time = sources[j].CurrentSourceTime + sources[j].LastSourceTime;
								if (sources[j + 1].CurrentSourceTime + sources[j + 1].LastSourceTime < Time)
									Time = sources[j + 1].CurrentSourceTime + sources[j + 1].LastSourceTime;
							}								
							break;
						}

						if (Time != events[0].Time)
							break;

						if (CheckFirstEventSource() != i)
                        {
							i++;
							if (i == NumberOfSources)
								i = 0;
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
								Console.WriteLine($"Zaplanowano zdarzenie przybycia do systemu na: {Time + PacketBreak}");
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
								Console.WriteLine($"Zaplanowano zdarzenie OPUSZCZENIA  systemu: {Time + ServiceTime}");
							}

							//if (sources[i].CurrentSourceTime + sources[i].LastSourceTime < events[0].Time)
							if (CheckSourcesOrEventTime())
							{
								if (queue.packets.Count > 1)
								{
									queue.surface += (queue.packets.Count - 1) * (sources[i].CurrentSourceTime + sources[i].LastSourceTime - Time);
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
								}
								Time = FindShortestTime();
							}
							else
							{
								if (queue.packets.Count > 1)
								{
									queue.surface += (queue.packets.Count - 1) * (events[0].Time - Time);
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
								}

								Time = events[0].Time;
							}

							_event = null;

							if (Time >= SimulationTime) Time = SimulationTime;
							i++;
							if (i == NumberOfSources)
								i = 0;
						}
						#endregion

						#region Main else if
						else if (_event != null && _event.Type == 2 && Time <= SimulationTime)
						{
							queue.delay += (Time - queue.packets[0].ArrivalTime - ServiceTime);
							queue.delayNumber++;
							Console.WriteLine("Obecny DELAY wynosi: " + queue.delay);

							Console.WriteLine($"Usunięto pakiet o: {Time} przybył on do symulacji o {queue.packets[0].ArrivalTime}");
							queue.RemovePacket(Time);
							if (queue.packets.Count <= 0)
							{
								if (events.Any())
									Time = events[0].Time;
								else
								{
									queue.emptyServer += Math.Min(sources[i].CurrentSourceTime + sources[i].LastSourceTime, SimulationTime) - Time;
									queue.surface += (queue.packets.Count * (sources[i].CurrentSourceTime + sources[i].LastSourceTime - Time));
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("KOLEJKA: " + queue.packets.Count);
									Time = sources[i].CurrentSourceTime + sources[i].LastSourceTime;
								}

							}
							else
							{
								AddEventToList(new Event(Time + ServiceTime, Event.Departure, queue.packets[0].SourceNumber));
								Console.WriteLine($"Zaplanowano zdarzenie opuszczenia systemu na: {Time + ServiceTime}");

								if (sources[i].CurrentSourceTime + sources[i].LastSourceTime < events[0].Time)
								{
									queue.surface += (queue.packets.Count - 1) * (sources[i].CurrentSourceTime + sources[i].LastSourceTime - Time);
									Console.WriteLine("SURFACE: " + queue.surface);
									Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
									Time = sources[i].CurrentSourceTime + sources[i].LastSourceTime;
									//break;
								}
								else
								{
									if (!(events[0].Time > SimulationTime))
									{
										queue.surface += (queue.packets.Count - 1) * (events[0].Time - Time);
										Console.WriteLine("SURFACE: " + queue.surface);
										Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
									}

									Time = events[0].Time;
								}
							}

							_event = null;

							if (Time >= SimulationTime)
								Time = SimulationTime;

							i++;
							if (i == NumberOfSources)
								i = 0;
						}
						#endregion

						#region Main else
						else
						{
							Console.WriteLine("WYWALONE PO CZASIE: " + queue.packets.Count);
							queue.packetDiscarded += queue.packets.Count;
							Time = SimulationTime;
							Console.WriteLine($"Czas symulacji zakończył się. Wynosi {Time}");
							break;
						}
						#endregion
					}

					break;
				}

			}			
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
					return true;
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

		/*public double findShortestTime()
		{
			double min = 0;
			double[] table = new double[2];
			int i = 0;
			foreach (Source s in sources)
			{
				double x = s.LastSourceTime + s.CurrentSourceTime;
				table[i] = x;

				if (table[i] < min)
				{
					min = table[i];
				}
				i++;
			}
			return min;
		}*/

		internal void ShowResults()
		{
			Console.WriteLine();
			Console.WriteLine("************************ RESULTS ************************");
			Console.WriteLine($"Total time: {Time}" );
			Console.WriteLine($"Mean number of packets in queue: {queue.surface/Time}" );
			Console.WriteLine($"Mean packet delay: {Math.Round((queue.delay/queue.delayNumber), 15)}" );
			Console.WriteLine($"Packets delayed: {Math.Round(queue.delay, 15)}" );
			Console.WriteLine($"Mean server load: {(Time - queue.emptyServer)/Time}" );
			Console.WriteLine($"Packet loss level: {(double)queue.packetDiscarded / queue.packetNumber * 100}%");
		}
	}
}
