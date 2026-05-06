using System;
using System.Linq;
using System.Collections.Generic;
using RecommendationSystemAI.Data;
using RecommendationSystemAI.Engine;

namespace RecommendationSystemAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Intelligent Recommendation System (Genetic Algorithm)...");

            // 1. تحديد مسار مجلد البيانات
            // ملاحظة هامة: تأكد أن مجلد HW_Data_S25 موجود في نفس مسار تشغيل البرنامج (bin/Debug/net...)
            string dataPath = @"HW_Data_S25\";
            var loader = new DataLoader();

            try
            {
                // 2. قراءة البيانات من ملفات الإكسل
                Console.WriteLine("Loading Data from Excel files...");
                var users = loader.LoadUsers(dataPath + "users.xlsx");
                var products = loader.LoadProducts(dataPath + "products.xlsx");
                var ratings = loader.LoadRatings(dataPath + "ratings.xlsx");
                var behaviors = loader.LoadBehaviors(dataPath + "behavior.xlsx");
                Console.WriteLine($"Successfully Loaded: {users.Count} Users, {products.Count} Products.");

                // 3. تهيئة محرك الخوارزمية الجينية
                var gaEngine = new RecommendationGA(behaviors, ratings, products);

                // 4. إعداد بارامترات الخوارزمية
                int targetUserId = 1; // يمكنك تغييره لتجربة مستخدمين مختلفين
                int populationSize = 50; // حجم المجتمع (50 مجموعة توصيات مختلفة)
                int generations = 20;    // عدد الأجيال
                double mutationRate = 0.05; // معدل الطفرة 5%

                Console.WriteLine($"\nOptimizing Recommendations for User ID: {targetUserId}");
                Console.WriteLine("--------------------------------------------------");

                // 5. توليد المجتمع الأولي
                var population = gaEngine.GenerateInitialPopulation(populationSize);

                // 6. حلقة التطور (Evolution Loop)
                for (int generation = 1; generation <= generations; generation++)
                {
                    // أ. حساب الـ Fitness Score لكل كروموسوم
                    foreach (var chromosome in population)
                    {
                        gaEngine.CalculateFitness(targetUserId, chromosome);
                    }

                    // ب. ترتيب الحلول من الأفضل (أعلى سكور) للأسوأ
                    population = population.OrderByDescending(c => c.FitnessScore).ToList();

                    // طباعة التطور لتشوف الديمو قدامك
                    Console.WriteLine($"Generation {generation}: Best Fitness Score = {population.First().FitnessScore}");

                    // ج. بناء الجيل الجديد
                    var newPopulation = new List<Chromosome>();

                    // مبدأ النخبوية (Elitism): نحافظ على أفضل حلين وننقلهم للجيل الجديد بدون أي تعديل لضمان عدم تراجع الأداء
                    newPopulation.Add(population[0]);
                    newPopulation.Add(population[1]);

                    var random = new Random();
                    // د. توليد باقي المجتمع
                    while (newPopulation.Count < populationSize)
                    {
                        // اختيار الآباء (عشوائياً من النصف الأفضل بالمجتمع لضمان الجودة)
                        var parent1 = population[random.Next(populationSize / 2)];
                        var parent2 = population[random.Next(populationSize / 2)];

                        // عملية التصالب
                        var child = gaEngine.Crossover(parent1, parent2);

                        // عملية الطفرة
                        gaEngine.Mutate(child, mutationRate);

                        newPopulation.Add(child);
                    }

                    population = newPopulation; // استبدال المجتمع القديم بالجديد
                }

                // 7. عرض النتيجة النهائية
                // بعد انتهاء الأجيال، أول عنصر هو الأفضل حتماً
                var bestSolution = population.OrderByDescending(c => c.FitnessScore).First();

                Console.WriteLine("\n==========================================");
                Console.WriteLine("🏆 Final Best Recommendation Generated 🏆");
                Console.WriteLine("==========================================");
                Console.WriteLine($"Target User: {targetUserId}");
                Console.WriteLine($"Final Fitness Score: {bestSolution.FitnessScore}");
                Console.WriteLine("Recommended Products:");

                foreach (var productId in bestSolution.ProductIds)
                {
                    var productInfo = products.FirstOrDefault(p => p.ProductId == productId);
                    string category = productInfo != null ? productInfo.Category : "Unknown";
                    string price = productInfo != null ? productInfo.Price.ToString("C") : "N/A";
                    Console.WriteLine($"- Product ID: {productId} | Category: {category} | Price: {price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error]: {ex.Message}");
                Console.WriteLine("Please make sure the 'HW_Data_S25' folder is in the correct output directory (bin/Debug/...) and the files are not open in Excel.");
            }

            Console.ReadLine(); // لمنع إغلاق شاشة الكونسول فوراً
        }
    }
}