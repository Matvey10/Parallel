using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
namespace ParallelLaba1
{
    class Program
    {
        static void Main(string[] args)
        {
            int size = Convert.ToInt32(Console.ReadLine());
            Step1 step1 = new Step1(size);
            step1.Run(size);
            Thread.Sleep(5000);
           /* for (int i = 0; i < step1.Results.Count; i++)
                Console.WriteLine($"сумма = {step1.Results[i].getResult()}");*/
        }

    }
}