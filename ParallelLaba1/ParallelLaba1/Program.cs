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
            object locker = new object(); 
            int size = Convert.ToInt32(Console.ReadLine());
            int[] X = new int[size];
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                X[i] = rand.Next(1, size);// size=20
            }
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine($"X[{i}] = {X[i]}");
            }
            List<Sum> Results = new List<Sum>();
            for (int i = 0; i < 4; i++)
            {
                Sum sum = new Sum(X);
                Results.Add(sum);
            }
                TaskManager threadManager = new TaskManager(4);
            threadManager.TaskManagerWorkDone += WorkDone;
            //int Res = 0;
            for (int i = 0; i < 4; i++)
            {
                //Sum sum = new Sum(X);
                var number = i;
                threadManager.AddTask(() => Results[number].sumValues(number * 5, (number * 5) + 4));//добавили задание по суммированию части массива
               
                //Results.Add(sum);
                //Console.WriteLine($"Res = {Res}");
            }
            for (int i = 0; i < Results.Count; i++)
            {
                Console.WriteLine($"сумма{i}={Results[i].getResult()}");
            }
            Console.WriteLine("Ожидание завершения");
            Console.ReadKey();
            //Console.WriteLine($"Res = {Res}");

        }

        public static void WorkDone()
        {
            Console.WriteLine("Менеджер завершил все задачи.");
            /*for (int i=0; i < Results.Count; i++)
                Console.WriteLine(Results[i]);*/
        }
    }
}
