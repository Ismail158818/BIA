using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystemAI.Engine
{
    // هاد الكلاس بيمثل "حل" واحد، يعني سلة توصيات مكونة من عدة منتجات
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
