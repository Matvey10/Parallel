using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ProcessTest2
{
    class Integer
    {
        public int value { get; set; }
        public int Id { get; set; }
        public Integer (int Id, int value)
        {
            this.Id = Id;
            this.value = value;
        }
        public Integer() { }
    }
    class ValuesContext : DbContext
    {
        public ValuesContext()
            : base("DbConnection")
        { }

        public DbSet<Integer> Values { get; set; }
    }
}
