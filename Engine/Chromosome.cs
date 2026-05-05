using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystemAI.Engine
{
   
    public class Chromosome
    {
        public List<int> ProductIds { get; set; }
        public double FitnessScore { get; set; }

        public Chromosome()
        {
            ProductIds = new List<int>();
            FitnessScore = 0;
        }
    }
}
