using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
namespace ParallelLaba1
{
    class Step1
    {
        public List<Sum> Results;
        public Step1(int size)
        {
            int[] X = new int[size];
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                X[i] = rand.Next(1, size);// size=100
                Console.WriteLine($"X[{i}] = {X[i]}");
            }
            Results = new List<Sum>();
            for (int i = 0; i < size / 5; i++)
            {
                Sum sum = new Sum(X);
                Results.Add(sum);
            }
        }
        public int sumResults()
        {
            int sum = 0;
            for (int i = 0; i < this.Results.Count; i++)
                sum += this.Results[i].getResult();
            return sum;
        }
        public void Run(int size)
        {
            TaskManager threadManager = new TaskManager(4);
            threadManager.TaskManagerWorkDone += WorkDone;
            //int Res = 0;
            for (int i = 0; i < size / 5; i++)
            {
                //Sum sum = new Sum(X);
                var number = i;
                threadManager.AddTask(() => Results[number].sumValues(number * 5, (number * 5) + 4));//добавили задание по суммированию части массива
                Console.WriteLine("Ожидание завершения");
                //Results.Add(sum);
                //Console.WriteLine($"Res = {Res}");
            }
        }
        public void WorkDone()
        {
            Console.WriteLine("Менеджер завершил все задачи.");
            for (int i = 0; i < this.Results.Count; i++)
                Console.WriteLine($"сумма = {this.Results[i].getResult()}");
            Console.WriteLine($"итоговая сумма = {this.sumResults()}");
            /*for (int i=0; i < Results.Count; i++)
                Console.WriteLine(Results[i]);*/
        }
        
    }
}