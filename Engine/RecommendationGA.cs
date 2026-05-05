using RecommendationSystemAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommendationSystemAI.Engine
{
   

    
    public class RecommendationGA
    {
        private List<Behavior> _behaviors;
        private List<Rating> _ratings;
        private List<Product> _allProducts;
        private Random _random;

        
        public RecommendationGA(List<Behavior> behaviors, List<Rating> ratings, List<Product> allProducts)
        {
            _behaviors = behaviors;
            _ratings = ratings;
            _allProducts = allProducts;
            _random = new Random();
        }

        
        public double CalculateFitness(int targetUserId, Chromosome chromosome)
        {
            double score = 0;

            foreach (var productId in chromosome.ProductIds)
            {
                
                var userBehavior = _behaviors.FirstOrDefault(b => b.UserId == targetUserId && b.ProductId == productId);

                if (userBehavior != null)
                {
                    if (userBehavior.Viewed) score += 1;     
                    if (userBehavior.Clicked) score += 3;    
                    if (userBehavior.Purchased) score += 5;  
                }

                
                var userRating = _ratings.FirstOrDefault(r => r.UserId == targetUserId && r.ProductId == productId);
                if (userRating != null)
                {
                    score += userRating.Value; 
                }
            }

            chromosome.FitnessScore = score;
            return score;
        }

        
        public List<Chromosome> GenerateInitialPopulation(int size)
        {
            var population = new List<Chromosome>();
            for (int i = 0; i < size; i++)
            {
                var chromosome = new Chromosome();
                
                chromosome.ProductIds = _allProducts.OrderBy(x => _random.Next()).Take(5).Select(p => p.ProductId).ToList();
                population.Add(chromosome);
            }
            return population;
        }

        
        public Chromosome Crossover(Chromosome parent1, Chromosome parent2)
        {
            var child = new Chromosome();
            
            child.ProductIds.AddRange(parent1.ProductIds.Take(2));
            child.ProductIds.AddRange(parent2.ProductIds.Skip(2).Take(3));
            return child;
        }

        
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