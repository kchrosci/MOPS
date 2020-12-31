using System;

namespace MOPS
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Welcome in simulator!");
			Queue q = new Queue();

			Console.WriteLine("Enter the value of beta: ");
			q.beta = double.Parse(Console.ReadLine());

			Console.WriteLine("Enter the simulation time: ");
			q.simulationTime = double.Parse(Console.ReadLine());
			
			Console.WriteLine("Starting simulation...");
						
			q.StartSimulation();
			Console.WriteLine("The simulation has been finished! Results:");
			q.ShowResults();
		}
	}
}
