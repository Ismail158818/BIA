using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using RecommendationSystemAI.Data;
using RecommendationSystemAI.Engine;
using RecommendationSystemAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommendationSystemAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        
        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            return View(new List<Product>());
        }

        [HttpPost]
        public IActionResult Index(int targetUserId)
        {
            try
            {
               
                string dataPath = System.IO.Path.Combine(_env.ContentRootPath, "HW_Data_S25") + System.IO.Path.DirectorySeparatorChar;

                var loader = new DataLoader();
                var users = loader.LoadUsers(dataPath + "users.xlsx");
                var products = loader.LoadProducts(dataPath + "products.xlsx");
                var ratings = loader.LoadRatings(dataPath + "ratings.xlsx");
                var behaviors = loader.LoadBehaviors(dataPath + "behavior.xlsx");

                var gaEngine = new RecommendationGA(behaviors, ratings, products);

                int populationSize = 50;
                int generations = 20;
                double mutationRate = 0.05;

                var population = gaEngine.GenerateInitialPopulation(populationSize);

                for (int generation = 1; generation <= generations; generation++)
                {
                    foreach (var chromosome in population)
                    {
                        gaEngine.CalculateFitness(targetUserId, chromosome);
                    }

                    population = population.OrderByDescending(c => c.FitnessScore).ToList();

                    var newPopulation = new List<Chromosome> { population[0], population[1] };
                    var random = new Random();

                    while (newPopulation.Count < populationSize)
                    {
                        var parent1 = population[random.Next(populationSize / 2)];
                        var parent2 = population[random.Next(populationSize / 2)];
                        var child = gaEngine.Crossover(parent1, parent2);
                        gaEngine.Mutate(child, mutationRate);
                        newPopulation.Add(child);
                    }
                    population = newPopulation;
                }

                var bestSolution = population.OrderByDescending(c => c.FitnessScore).First();

                
                var recommendedProducts = new List<Product>();
                foreach (var id in bestSolution.ProductIds)
                {
                    var p = products.FirstOrDefault(x => x.ProductId == id);
                    if (p != null) recommendedProducts.Add(p);
                }

               
                ViewBag.FitnessScore = bestSolution.FitnessScore;
                ViewBag.UserId = targetUserId;

               
                var userHistory = new List<Product>();
                var pastBehaviors = behaviors.Where(b => b.UserId == targetUserId && (b.Purchased || b.Clicked || b.Viewed)).ToList();

                foreach (var b in pastBehaviors)
                {
                    var p = products.FirstOrDefault(x => x.ProductId == b.ProductId);
                    if (p != null && !userHistory.Any(h => h.ProductId == p.ProductId))
                    {
                        userHistory.Add(p);
                    }
                }
                ViewBag.UserHistory = userHistory;
               

                return View(recommendedProducts);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "ÕœÀ Œÿ√:  √ﬂœ „‰ ÊÃÊœ „Ã·œ HW_Data_S25 ›Ì «·„”«— «·’ÕÌÕ Ê≈€·«ﬁ „·›«  «·≈ﬂ”·. «· ›«’Ì·: " + ex.Message;
                return View(new List<Product>());
            }
        }
    }
}