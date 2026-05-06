using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using RecommendationSystemAI.Models; // تأكد أن مجلد الـ Models بنفس الاسم

namespace RecommendationSystemAI.Data
{
    public class DataLoader
    {
        public DataLoader()
        {
            // هاد السطر إجباري لتجنب أخطاء الترميز عند قراءة الإكسل في نسخ الدوت نت الحديثة
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public List<Behavior> LoadBehaviors(string filePath)
        {
            var behaviors = new List<Behavior>();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // الآن AsDataSet ستعمل بشكل طبيعي
                    var result = reader.AsDataSet();
                    var table = result.Tables[0]; // قراءة أول شيت بالملف

                    for (int i = 1; i < table.Rows.Count; i++) // تجاوز السطر الأول (الهيدر)
                    {
                        behaviors.Add(new Behavior
                        {
                            UserId = Convert.ToInt32(table.Rows[i][0]),
                            ProductId = Convert.ToInt32(table.Rows[i][1]),
                            Viewed = table.Rows[i][2].ToString() == "1",
                            Clicked = table.Rows[i][3].ToString() == "1",
                            Purchased = table.Rows[i][4].ToString() == "1"
                        });
                    }
                }
            }
            return behaviors;
        }

        // دالة قراءة المستخدمين
        public List<User> LoadUsers(string filePath)
        {
            var users = new List<User>();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];
                    for (int i = 1; i < table.Rows.Count; i++) // تجاوز الهيدر
                    {
                        users.Add(new User
                        {
                            UserId = Convert.ToInt32(table.Rows[i][0]),
                            Age = Convert.ToInt32(table.Rows[i][1]),
                            Location = table.Rows[i][2].ToString()
                        });
                    }
                }
            }
            return users;
        }

        // دالة قراءة المنتجات
        public List<Product> LoadProducts(string filePath)
        {
            var products = new List<Product>();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        products.Add(new Product
                        {
                            ProductId = Convert.ToInt32(table.Rows[i][0]),
                            Category = table.Rows[i][1].ToString(),
                            Price = Convert.ToDouble(table.Rows[i][2])
                        });
                    }
                }
            }
            return products;
        }

        // دالة قراءة التقييمات
        public List<Rating> LoadRatings(string filePath)
        {
            var ratings = new List<Rating>();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        ratings.Add(new Rating
                        {
                            UserId = Convert.ToInt32(table.Rows[i][0]),
                            ProductId = Convert.ToInt32(table.Rows[i][1]),
                            Value = Convert.ToInt32(table.Rows[i][2])
                        });
                    }
                }
            }
            return ratings;
        }
    }
}