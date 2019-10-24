using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace ParallelLaba1
{
    class Step2
    {
        public List<Sum> Results;
        public double mx;
        public double Dx;
        private int size;
        public readonly int ThreadsCount = 10;
        //TaskManager threadManager;
        private ConcurrentQueue<Action> _actionsQueue;
        public Step2(int size, double mx, int[] X)
        {
            this.mx = mx;
            this.size = size;
            Results = new List<Sum>();
            for (int i = 0; i < size / 1000; i++)
            {
                Sum sum = new Sum(X);
                Results.Add(sum);
            }
        }
        public void Run(int size)//вынеси в отд поле
        {
            TaskManager threadManager = new TaskManager(10);
            threadManager.TaskManagerWorkDone += ComputeDxDone;
            _actionsQueue = new ConcurrentQueue<Action>();
            for (int i = 0; i < size / 1000; i++)
            {
                var number = i;
                _actionsQueue.Enqueue(() => Results[number].squareDifValues(number * 1000, (number * 1000) + 999, mx));
                //threadManager.AddTask(() => Results[number].sumValues(number * 1000, (number * 1000) + 999));//добавили задание по суммированию части массива
                //threadManager.AddTask(() => Results[number].squareDifValues(number * 1000, (number * 1000) + 999, mx));//добавили задание по суммированию части массива
                //Console.WriteLine("Ожидание завершения");
            }
            threadManager.AddTasks(ref _actionsQueue);
        }
        public ulong sumResults()
        {
            ulong sum = 0;
            for (int i = 0; i < this.Results.Count; i++)
                sum += (ulong)this.Results[i].getSumDifInSquare();
            return sum;
        }
        public void ComputeDxDone()
        {
            Console.WriteLine("Менеджер завершил работу по вычислению выборочной дисперсии");
            for (int i = 0; i < this.Results.Count; i++)
                Console.WriteLine($"сумма = {this.Results[i].getSumDifInSquare()}");
            Console.WriteLine($"итоговая сумма = {this.sumResults()}");
            Dx = (double)(this.sumResults()) / size;
            /*foreach (var thread in threadManager.getThreads())
            {
                thread.Terminate();
            }*/
            Console.WriteLine($"мат ожидание = {mx}; дисперсия = {Dx}");
        }
    }
}
