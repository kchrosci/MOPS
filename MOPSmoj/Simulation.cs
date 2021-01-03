using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOPS
{
	class Simulation
	{
		#region Properties

		public double LastTimeOFF { get; set; } = 0;
		public double LastTimeON { get; set; } = 0;
		public double PacketBreak { get; set; } = 0.1;
		public double ServiceTime { get; set; } = 0.5;
		public double Beta { get; set; } = 1;
		public double TimeON { get; set; }
		public double TimeOFF { get; set; }
		public double Time { get; set; }
		public double SimulationTime { get; set; } = 5;
		public int QueueLength { get; set; } = 5;
		public double CurrentSourceTime { get; set; }
		public double LastSourceTime { get; set; }


		public List<Event> events = new List<Event>();

		Source source;
		Queue queue;
		#endregion

		internal void StartSimulation()
		{	
			source = new Source();
			queue = new Queue();

			Time = 0;
			
			while (Time < SimulationTime)
			{
				Event _event = new Event(Time, Event.Arrival);
				
				AddEventToList(_event);

				LastTimeOFF += TimeOFF;
				LastTimeON += TimeON;

				source.TimeGenerator(Beta);
				TimeON = source.TimeON;
				TimeOFF = source.TimeOFF;

				CurrentSourceTime = TimeOFF + TimeON;
				LastSourceTime = LastTimeOFF + LastTimeON;

				while (CurrentSourceTime > Time - LastSourceTime && events.Count != 0)
				{
					if (Time > SimulationTime) break;

					if ((events.Count == 0 && queue.packets.Count == 0))
					{
						Time = CurrentSourceTime + LastSourceTime;
						break;
					}
					
					if(events.Any() && events[0].Time <= SimulationTime)
					{
						_event = TakeNextEvent();
					}

					Console.WriteLine("\nCurrent time of the server is: " + Time);

					#region Main if
					if (_event != null && _event.Type == 1 && Time < SimulationTime)
					{
						queue.packetNumber++;
						Console.WriteLine("Number of packet: " + queue.packetNumber);

						if (Time + PacketBreak <= TimeON + LastSourceTime && Time + PacketBreak <= SimulationTime)
						{
							AddEventToList(new Event(Time + PacketBreak, Event.Arrival));
							Console.WriteLine($"Zaplanowano zdarzenie przybycia do systemu na: {Time + PacketBreak}");
						}
							
						if(QueueLength >= queue.packets.Count)
						{
							queue.AddPacket(new Packet(Time, ServiceTime));
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
							AddEventToList(new Event(Time + ServiceTime, Event.Departure));
							Console.WriteLine($"Zaplanowano zdarzenie OPUSZCZENIA  systemu: {Time + ServiceTime}");
						}

						if (CurrentSourceTime + LastSourceTime < events[0].Time)
						{
							if (queue.packets.Count > 1)
                            {
								queue.surface += (queue.packets.Count - 1) * (CurrentSourceTime + LastSourceTime - Time);
								Console.WriteLine("SURFACE: " + queue.surface);
								Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
							}
							Time = CurrentSourceTime + LastSourceTime;
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
								queue.emptyServer += Math.Min(CurrentSourceTime + LastSourceTime, SimulationTime) - Time;
								queue.surface += (queue.packets.Count * (CurrentSourceTime + LastSourceTime - Time));
								Console.WriteLine("SURFACE: " + queue.surface);
								Console.WriteLine("KOLEJKA: " + queue.packets.Count);
								Time = CurrentSourceTime + LastSourceTime;
							}
									
						}
						else
						{
							AddEventToList(new Event(Time + ServiceTime, Event.Departure));
							Console.WriteLine($"Zaplanowano zdarzenie opuszczenia systemu na: {Time + ServiceTime}");

							if (CurrentSourceTime + LastSourceTime < events[0].Time)
							{
								queue.surface += (queue.packets.Count - 1) * (CurrentSourceTime + LastSourceTime - Time);
								Console.WriteLine("SURFACE: " + queue.surface);
								Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
								Time = CurrentSourceTime + LastSourceTime;
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
