using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystemAI.Models
{
    public class Rating
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Value { get; set; } // من 1 لـ 5
    }
}
