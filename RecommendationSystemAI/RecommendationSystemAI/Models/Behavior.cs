using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystemAI.Models
{
    public class Behavior
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public bool Viewed { get; set; }
        public bool Clicked { get; set; }
        public bool Purchased { get; set; }
    }
}
