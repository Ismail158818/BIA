using RecommendationSystemAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommendationSystemAI.Engine
{
   

    // هاد الكلاس هو المحرك الأساسي للخوارزمية
    public class RecommendationGA
    {
        private List<Behavior> _behaviors;
        private List<Rating> _ratings;
        private List<Product> _allProducts;
        private Random _random;

        // الباني (Constructor) بياخد الداتا اللي قرأناها من الإكسل
        public RecommendationGA(List<Behavior> behaviors, List<Rating> ratings, List<Product> allProducts)
        {
            _behaviors = behaviors;
            _ratings = ratings;
            _allProducts = allProducts;
            _random = new Random();
        }

        // 1. دالة حساب التقييم (Fitness Function) - الأهم بالمشروع
        public double CalculateFitness(int targetUserId, Chromosome chromosome)
        {
            double score = 0;

            foreach (var productId in chromosome.ProductIds)
            {
                // منشوف إذا المستخدم إلو أي تفاعل سابق مع هاد المنتج
                var userBehavior = _behaviors.FirstOrDefault(b => b.UserId == targetUserId && b.ProductId == productId);

                if (userBehavior != null)
                {
                    if (userBehavior.Viewed) score += 1;     // الشوفة بتعطي نقطة
                    if (userBehavior.Clicked) score += 3;    // النقر بيعطي 3 نقاط
                    if (userBehavior.Purchased) score += 5;  // الشراء هو الأهم وبيعطي 5 نقاط
                }

                // منشوف إذا مقيم المنتج من قبل
                var userRating = _ratings.FirstOrDefault(r => r.UserId == targetUserId && r.ProductId == productId);
                if (userRating != null)
                {
                    score += userRating.Value; // منضيف قيمة التقييم (من 1 لـ 5) للسكور
                }
            }

            chromosome.FitnessScore = score;
            return score;
        }

        // 2. توليد المجتمع الأولي (عشوائياً حسب المقالة)
        public List<Chromosome> GenerateInitialPopulation(int size)
        {
            var population = new List<Chromosome>();
            for (int i = 0; i < size; i++)
            {
                var chromosome = new Chromosome();
                // اختيار 5 منتجات عشوائية لكل حل 
                chromosome.ProductIds = _allProducts.OrderBy(x => _random.Next()).Take(5).Select(p => p.ProductId).ToList();
                population.Add(chromosome);
            }
            return population;
        }

        // 3. عملية التصالب (Crossover)
        // دمج حلين "أبوين" لإنتاج حل "ابن" جديد
        public Chromosome Crossover(Chromosome parent1, Chromosome parent2)
        {
            var child = new Chromosome();
            // نأخذ أول منتجين من الأب الأول والباقي من الأب الثاني
            child.ProductIds.AddRange(parent1.ProductIds.Take(2));
            child.ProductIds.AddRange(parent2.ProductIds.Skip(2).Take(3));
            return child;
        }

        // 4. عملية الطفرة (Mutation)
        // تغيير منتج واحد بشكل عشوائي للحفاظ على التنوع وتجنب التكرار الممل
        public void Mutate(Chromosome chromosome, double mutationRate)
        {
            if (_random.NextDouble() < mutationRate)
            {
                int indexToChange = _random.Next(chromosome.ProductIds.Count);
                int newProductId = _allProducts[_random.Next(_allProducts.Count)].ProductId;
                chromosome.ProductIds[indexToChange] = newProductId;
            }
        }
    }
}