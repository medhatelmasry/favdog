using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace redis4u.Models
{
    public class AnimalVote : Animal
    {
        public int Count { get; set; }
        public int Percent { get; set; }
    }
}
