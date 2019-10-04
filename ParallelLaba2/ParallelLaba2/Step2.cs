using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelLaba1
{
    class Step2
    {
        public List<Sum> Results;
        //private Step1 step1;
        public int mx;
        public int Dx;
        private int size;
        public Step2(int size, int mx, int[] X)
        {
            this.mx = mx;
            this.size = size;
            Results = new List<Sum>();
            for (int i = 0; i < size / 5; i++)
            {
                Sum sum = new Sum(X);
                Results.Add(sum);
            }
        }
        public void Run(int size)//вынеси в отд поле
        {
            TaskManager threadManager = new TaskManager(4);
            threadManager.TaskManagerWorkDone += ComputeDxDone;
            //int Res = 0;
            for (int i = 0; i < size / 5; i++)
            {
                var number = i;
                threadManager.AddTask(() => Results[number].squareDifValues(number * 5, (number * 5) + 4, mx));//добавили задание по суммированию части массива
                Console.WriteLine("Ожидание завершения");
            }

        }
        public int sumResults()
        {
            int sum = 0;
            for (int i = 0; i < this.Results.Count; i++)
                sum += this.Results[i].getSumDifInSquare();
            return sum;
        }
        public void ComputeDxDone()
        {
            Console.WriteLine("Менеджер завершил работу по вычислению выборочной дисперсии");
            for (int i = 0; i < this.Results.Count; i++)
                Console.WriteLine($"сумма = {this.Results[i].getSumDifInSquare()}");
            Console.WriteLine($"итоговая сумма = {this.sumResults()}");
            Dx = this.sumResults() / size;
        }
    }
}
