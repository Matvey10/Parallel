using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeTest
{
    class ResultsList
    {
        private List<int> Results;
        public ResultsList()
        {
            Results = new List<int>();
        }
        public List<int> getResults()
        {
            return Results;
        }
        public void AddResult(int res)
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
    }
}

