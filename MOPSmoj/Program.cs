using System;

namespace MOPS
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Welcome in simulator!");
			Simulation simulation = new Simulation();
			
			Console.WriteLine("Enter the value of beta: ");
			//simulation.Beta = double.Parse(Console.ReadLine());
						
			Console.WriteLine("Enter the simulation time: ");
			//simulation.SimulationTime = double.Parse(Console.ReadLine());
			
			Console.WriteLine("Starting simulation...");
					
			simulation.StartSimulation();
			Console.WriteLine("The simulation has been finished! Results:");
			simulation.ShowResults();
		}
	}
}
