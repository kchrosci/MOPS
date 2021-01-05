using System;
using System.Collections.Generic;
using System.Text;

namespace MOPS
{
    class Source
    {
        public double TimeON { get; set; } = 0; 
        public double TimeOFF { get; set; } = 0;
        public double LastTimeOFF { get; set; } = 0;
        public double LastTimeON { get; set; } = 0;
        public double CurrentSourceTime { get; set; } = 0;
        public double LastSourceTime { get; set; } = 0;

        public void TimeGenerator(double betaON, double betaOFF, int number)
        {
            Random random = new Random();
            TimeON = -(betaON) * Math.Log(1 - random.NextDouble());
            TimeOFF = -(betaOFF) * Math.Log(1 - random.NextDouble());
            Console.WriteLine($"\nSOURCE: {number} * ***** TIME OF STATES ON/OFF HAS BEEN GENERATED *******\nTime ON: {TimeON}\nTime OFF: {TimeOFF}");
		}
    }
}
