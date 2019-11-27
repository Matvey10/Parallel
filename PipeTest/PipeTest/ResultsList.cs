using System;
using System.Collections.Generic;
using System.Collections;

namespace PipeTest
{
    class ResultsList
    {
        private List<double> Results;
        public ResultsList()
        {
            Results = new List<double>();
        }
        public IEnumerator GetEnumerator()
        {
            return Results.GetEnumerator();
        }
        public List<double> getResults()
        {
            return Results;
        }
        public void AddResult(double res)
        {
            try
            {
                Results.Add(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Clear()
        {
            Results.Clear();
        }
    }
}

