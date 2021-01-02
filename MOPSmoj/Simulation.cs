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
		public double packetBreak = 0.3;
		public double serviceTime = 0.9;

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
				

				while (TimeON + TimeOFF > Time - lastTimeOFF - lastTimeON)
				{
					if (Time >= SimulationTime) break;

					if ((events.Count == 0 && queue.packets.Count == 0))
					{
						Time = TimeOFF + TimeON + lastTimeOFF+lastTimeON;
						break;
					}

					if(Time - lastTimeON - lastTimeOFF <= TimeON + TimeOFF)
					{
						if(events.Any())
						{
							_event = TakeNextEvent();
						}

						Console.WriteLine("Current time of the server is: " + Time);

						if (_event.Type == 1)
						{
							queue.packetNumber++;
							Console.WriteLine("NUMER PAKIETU: " + queue.packetNumber);

							//currentTimeOn = Time - lastTimeOFF - lastTimeON;
							if (Time + packetBreak < TimeON + lastTimeOFF + lastTimeON )
							{
								AddEventToList(new Event(Time + packetBreak, Event.Arrival));
								Console.WriteLine($"Zaplanowano zdarzenie przybycia do systemu na: {Time + packetBreak}");
							}
							
							if(QueueLength > queue.packets.Count)
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
								Time = TimeOFF + TimeON+ lastTimeOFF + lastTimeON ;
							}
							else
							{
								Time = events[0].Time;
							}
							

						}
						// jezeli pakiet jest typu Departure
						else if (_event.Type == 2)
						{
							queue.delay += Time - queue.packets[0].ArrivalTime - serviceTime;
							queue.delayNumber++;

							Console.WriteLine($"Usunięto pakiet o: {Time} przybył on do symulacji o {queue.packets[0].ArrivalTime}");
							queue.RemovePacket(Time);
							if (queue.packets.Count <= 0) 
							{
								if(events.Any())
									Time = events[0].Time;
								else
									Time = TimeOFF + TimeON + lastTimeON + lastTimeOFF;
							}
							else
							{
								AddEventToList(new Event(Time + serviceTime, Event.Departure));
								Console.WriteLine($"Zaplanowano zdarzenie opuszczenia systemu na: {Time + serviceTime}");

								if (TimeOFF + TimeON + lastTimeON + lastTimeOFF < events[0].Time)
								{
									Time = TimeOFF + TimeON + lastTimeON + lastTimeOFF;
								}
								else
								{
									Time = events[0].Time;
								}
							}
						}
						else
						{
							Console.WriteLine("Test");
							//Time = TimeOFF + TimeON;
						}
					}
					else
					{
						Time = events[0].Time;
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
			Console.WriteLine($"Mean packet delay: {queue.delay/queue.delayNumber}" );
			Console.WriteLine($"Packets delayed: {Time}" );
			Console.WriteLine($"Mean server load: {Time}" );
			Console.WriteLine($"Mean packet loss: {(double)queue.packetDiscarded / queue.packetNumber * 100}%");

		}
	}
}
