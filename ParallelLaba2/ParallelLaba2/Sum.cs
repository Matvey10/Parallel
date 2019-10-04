using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
namespace ParallelLaba1
{
    class Sum
    {
        static readonly Random _rnd = new Random();
        private int result;
        private int[] a;
        public Sum (int[] a)
        {
            this.a = a;
        }
        public void sumValues(int id1, int id2)
        {
            for (int i = id1; i <= id2; i++)
                result += a[i];
            var workTime = _rnd.Next(150, 550);
            Thread.Sleep(workTime);
        }
        public int getResult()
        {
            return result;
        }
    }
}
