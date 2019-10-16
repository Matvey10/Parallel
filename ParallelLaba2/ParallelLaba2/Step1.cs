﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
namespace ParallelLaba1
{
    class Step1
    {
        private double mx;
        private int size;
        public List<Sum> Results;
        public Step1(int size)
        {
            this.size = size;
            int[] X = new int[size];
            Random rand = new Random();
            for (var i = 0; i < size; i++)
            {
                X[i] = rand.Next(1, 1000);
                //Console.WriteLine($"X[{i}] = {X[i]}");
            }
            Results = new List<Sum>();
            for (int i = 0; i < size / 1000; i++)
            {
                Sum sum = new Sum(X);
                Results.Add(sum);
            }
        }

        public double getMx ()
        {
            return mx;
        }

        public ulong sumResults()
        {
            ulong sum = 0;
            for (int i = 0; i < this.Results.Count; i++)
                sum += (ulong)this.Results[i].getResult();
            return sum;
        }
        public void Run(int size)
        {
            TaskManager threadManager = new TaskManager(10);
            threadManager.TaskManagerWorkDone += WorkDone;
            //threadManager.TaskManagerWorkDone += startStep2;          
            for (int i = 0; i < size / 1000; i++)
            {
                //Sum sum = new Sum(X);
                var number = i;
                threadManager.AddTask(() => Results[number].sumValues(number * 1000, (number * 1000) + 999));//добавили задание по суммированию части массива
               // Console.WriteLine("Ожидание завершения");
                //Results.Add(sum);
                //Console.WriteLine($"Res = {Res}");
            }
        }
        public void WorkDone()
        {
            Console.WriteLine("Менеджер завершил все задачи.");
            /*for (int i = 0; i < this.Results.Count; i++)
                Console.WriteLine($"сумма = {this.Results[i].getResult()}");*/
            Console.WriteLine($"итоговая сумма = {this.sumResults()}");
            mx = (double)(this.sumResults()) / size;
            startStep2();
            /*for (int i=0; i < Results.Count; i++)
                Console.WriteLine(Results[i]);*/
        }
        public void startStep2 ()
        {
            Step2 step2 = new Step2(size, this.getMx(), this.Results[0].a);
            step2.Run(size);
           // Thread.Sleep(5000);
        }
        
    }
}