using AllTheBeans.Domain.Entities;
using AllTheBeans.Infrastructure.Persistence;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AllTheBeans.Infrastructure.Seeding;

public static class BeansJsonSeeder
{
    public static void SeedFromFile(AllTheBeansDbContext dbContext, string filePath)
    {
        if (dbContext.Beans.Any()) return; // already seeded
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        var records = JsonSerializer.Deserialize<List<BeansJsonRecord>>(json);
        if (records == null) return;

        foreach (var rec in records)
        {
            string currencyCode = "GBP";
            string costStr = rec.Cost;
            if (costStr.StartsWith("£"))
            {
                costStr = costStr[1..];
                currencyCode = "GBP";
            }
            else if (costStr.StartsWith("$"))
            {
                costStr = costStr[1..];
                currencyCode = "USD";
            }
            else if (costStr.StartsWith("€"))
            {
                costStr = costStr[1..];
                currencyCode = "EUR";
            }
            if (!decimal.TryParse(costStr, out decimal amount))
            {
                amount = 0;
            }

            var bean = new Bean
            {
                Name = rec.Name,
                Colour = rec.colour,
                Country = rec.Country,
                Description = rec.Description.Trim(),
                ImageUrl = rec.Image,
                Cost = new Domain.ValueObjects.Money(amount, currencyCode)
            };
            dbContext.Beans.Add(bean);
        }
        dbContext.SaveChanges();
    }
}
