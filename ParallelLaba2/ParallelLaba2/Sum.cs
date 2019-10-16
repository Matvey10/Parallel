using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
namespace ParallelLaba1
{
    class Sum
    {
       // static readonly Random _rnd = new Random();
        private int resultOfSum;
        private double sumDifInSquare;
        public int[] a;
        public Sum (int[] a)
        {
            this.a = a;
        }
        public void sumValues(int id1, int id2)
        {
            for (int i = id1; i <= id2; i++)
                resultOfSum += a[i];
        }
        public void squareDifValues(int id1, int id2, double mx)
        {
            for (int i = id1; i <= id2; i++)
                sumDifInSquare += Math.Pow(a[i]-mx, 2);
        }
        public int getResult()
        {
            return resultOfSum;
        }
        public double getSumDifInSquare()
        {
            return sumDifInSquare;
        }
    }
}
