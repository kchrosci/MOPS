using System;

namespace MOPS
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Welcome in simulator!");
			Simulation simulation = new Simulation();

			Console.WriteLine("Enter the value of betaON: ");
			simulation.BetaON = double.Parse(Console.ReadLine());
			
			Console.WriteLine("Enter the value of betaOFF: ");
			simulation.BetaOFF = double.Parse(Console.ReadLine());

			Console.WriteLine("Enter the simulation time: ");
			simulation.SimulationTime = double.Parse(Console.ReadLine());

			Console.WriteLine("Enter packet service time: ");
			simulation.ServiceTime = double.Parse(Console.ReadLine());

			Console.WriteLine("Enter packet break time: ");
			simulation.PacketBreak = double.Parse(Console.ReadLine());

			Console.WriteLine("Enter the length of packet queue: ");
			simulation.QueueLength = int.Parse(Console.ReadLine());

			Console.WriteLine("Enter the number of sources: ");
			simulation.NumberOfSources = int.Parse(Console.ReadLine());

			Console.WriteLine("Starting simulation...");
			simulation.StartSimulation();
			Console.WriteLine("\nThe simulation has been finished! Results:");
			simulation.ShowResults();
		}
	}
}
