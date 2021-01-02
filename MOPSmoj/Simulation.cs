using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOPS
{
	class Simulation
	{

		public double lastTimeOFF = 0;
		public double lastTimeON = 0;
		public double packetBreak = 0.1;
		public double serviceTime = 0.5;

		public double Beta { get; set; } = 1;
		public double Delay { get; set; }
		public double TimeON { get; set; }
		public double TimeOFF { get; set; }
		public double currentTimeOn { get; set; } = 0;
		public double Time { get; set; }
		public double SimulationTime { get; set; } = 5;
		public int QueueLength { get; set; } = 5;

		public List<Event> events = new List<Event>();

		Source source;
		Queue queue;

		internal void StartSimulation()
		{	
			source = new Source();
			queue = new Queue();

			Time = 0;
			
			while (Time < SimulationTime)
			{
				
				Event _event = new Event(Time, Event.Arrival); // czas, typ
				AddEventToList(_event);

				lastTimeOFF += TimeOFF;
				lastTimeON += TimeON;

				source.TimeGenerator(Beta);
				TimeON = source.TimeON;
				TimeOFF = source.TimeOFF;
				//TimeOFF = 1.54;
				//TimeON = 1.28;
				

				while (TimeON + TimeOFF > Time - lastTimeOFF - lastTimeON && events.Count != 0)
				{
					if (Time > SimulationTime) break;

					if ((events.Count == 0 && queue.packets.Count == 0))
					{
						Time = TimeOFF + TimeON + lastTimeOFF+lastTimeON;
						break;
					}
					
					if(events.Any() && events[0].Time <= SimulationTime)
					{
						_event = TakeNextEvent();
					}

					Console.WriteLine("\nCurrent time of the server is: " + Time);

					if (_event != null && _event.Type == 1 && Time < SimulationTime)
					{
						queue.packetNumber++;
						Console.WriteLine("NUMER PAKIETU: " + queue.packetNumber);

						//currentTimeOn = Time - lastTimeOFF - lastTimeON;
						if (Time + packetBreak <= TimeON + lastTimeOFF + lastTimeON && Time + packetBreak <= SimulationTime)
						{
							AddEventToList(new Event(Time + packetBreak, Event.Arrival));
							Console.WriteLine($"Zaplanowano zdarzenie przybycia do systemu na: {Time + packetBreak}");
						}
							
						if(QueueLength >= queue.packets.Count)
						{
							queue.AddPacket(new Packet(Time, serviceTime));
						}
						else
						{
							queue.packetDiscarded++;
							Console.WriteLine("Packet DISCARDED! NUMER: " + queue.packetDiscarded);								
						}
						//planuje opuszczenie systemu dopiero gdy zostal 1 
						if (queue.packets.Count > 1) { }
						else
						{
							AddEventToList(new Event(Time + serviceTime, Event.Departure));
							Console.WriteLine($"Zaplanowano zdarzenie OPUSZCZENIA  systemu: {Time + serviceTime}");
						}

						if (TimeOFF + TimeON + lastTimeOFF + lastTimeON < events[0].Time)
						{
							if (queue.packets.Count > 1)
                            {
								queue.surface += (queue.packets.Count - 1) * (TimeOFF + TimeON + lastTimeOFF + lastTimeON - Time);
								Console.WriteLine("SURFACE: " + queue.surface);
								Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
							}
								
							Time = TimeOFF + TimeON+ lastTimeOFF + lastTimeON;
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

						if (Time >= SimulationTime)
							Time = SimulationTime;

					}
					// jezeli pakiet jest typu Departure
					else if (_event != null && _event.Type == 2 && Time <= SimulationTime)
					{ 
						queue.delay += (Time - queue.packets[0].ArrivalTime - serviceTime);
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
								queue.emptyServer += Math.Min(TimeOFF + TimeON + lastTimeON + lastTimeOFF, SimulationTime) - Time;
								queue.surface += (queue.packets.Count * (TimeOFF + TimeON + lastTimeOFF + lastTimeON - Time));
								Console.WriteLine("SURFACE: " + queue.surface);
								Console.WriteLine("KOLEJKA: " + queue.packets.Count);
								Time = TimeOFF + TimeON + lastTimeON + lastTimeOFF;
							}
									
						}
						else
						{
							AddEventToList(new Event(Time + serviceTime, Event.Departure));
							Console.WriteLine($"Zaplanowano zdarzenie opuszczenia systemu na: {Time + serviceTime}");

							if (TimeOFF + TimeON + lastTimeON + lastTimeOFF < events[0].Time)
							{
								queue.surface += (queue.packets.Count - 1) * (TimeOFF + TimeON + lastTimeOFF + lastTimeON - Time);
								Console.WriteLine("SURFACE: " + queue.surface);
								Console.WriteLine("KOLEJKA: " + (queue.packets.Count - 1));
								Time = TimeOFF + TimeON + lastTimeON + lastTimeOFF;
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
					else
					{
						Console.WriteLine("WYWALONE PO CZASIE: " + queue.packets.Count);
						queue.packetDiscarded += queue.packets.Count;

						//queue.surface += queue.packets.Count * (SimulationTime - Time);
						//Console.WriteLine("SURFACE: " + queue.surface);

						Time = SimulationTime;
						Console.WriteLine($"Czas symulacji zakończył się. Wynosi {Time}");
						break;
					}
					
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
			//umieszczenie eventu na liscie zdarzeń
			events.Insert(i, _event);			
		}

		internal void ShowResults()
		{
			Console.WriteLine("************************ RESULTS ************************");
			Console.WriteLine($"Total time: {Time}" );
			Console.WriteLine($"Mean number of packets in queue: {queue.surface/Time}" );
			Console.WriteLine($"Mean packet delay: {Math.Round((queue.delay/queue.delayNumber), 15)}" );
			//Console.WriteLine($"Packets delayed: {Math.Round(queue.delay, 15)}" );
			Console.WriteLine($"Mean server load: {(Time - queue.emptyServer)/Time}" );
			Console.WriteLine($"Packet loss level: {(double)queue.packetDiscarded / queue.packetNumber * 100}%");

		}
	}
}
