using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        public Question FirstQuestion { get; set; }

        public ICollection<Solution> Solutions { get; set; }
    }
}
