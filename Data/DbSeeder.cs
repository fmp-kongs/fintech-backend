using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FintechBackend.Models;
using FintechBackend.Models.Enums;

namespace FintechBackend.Data;

public static class DbSeeder
{
    public static void SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.Migrate();
        var passwordHasher = new PasswordHasher<User>();
        var rand = new Random();

        // Seed Users
        if (!db.Users.Any())
        {
            var adminUserOptions = configuration.GetSection("AdminUser").Get<AdminUserOptions>();
            if (adminUserOptions == null)
                throw new InvalidOperationException("AdminUser section is missing in appsettings.json");

            var admin = new User
            {
                Name = adminUserOptions.Name,
                Email = adminUserOptions.Email,
                Role = adminUserOptions.Role,
                PasswordHash = passwordHasher.HashPassword(null!, adminUserOptions.Password)
            };

            var user = new User
            {
                Name = "Alice",
                Email = "alice@fintech.com",
                Role = "User",
                PasswordHash = passwordHasher.HashPassword(null!, "User@123")
            };

            db.Users.AddRange(admin, user);

            // Add bulk demo users
            var bulkUsers = new List<User>();
            for (int i = 1; i <= 100; i++)
            {
                var u = new User
                {
                    Name = $"User{i}",
                    Email = $"user{i}@fintech.com",
                    Role = "User",
                    PasswordHash = passwordHasher.HashPassword(null!, "Password@123")
                };
                bulkUsers.Add(u);
            }
            db.Users.AddRange(bulkUsers);
            db.SaveChanges();
        }

        // Seed Products
        if (!db.Products.Any())
        {
            var products = new List<Product>();
            for (int i = 1; i <= 50; i++)
            {
                products.Add(new Product
                {
                    Name = $"Product {i}",
                    Description = $"Description for Product {i}",
                    Price = (decimal)(rand.Next(100, 50000) / 100.0),
                    CreatedAt = DateTime.UtcNow.AddDays(-rand.Next(0, 365))
                });
            }
            db.Products.AddRange(products);
            db.SaveChanges();
        }

        // Seed Transactions
        if (!db.Transactions.Any())
        {
            var users = db.Users.ToList();
            var products = db.Products.ToList();
            var statusValues = Enum.GetValues<TransactionStatus>();

            var transactions = new List<Transaction>();
            for (int i = 0; i < 5000; i++)
            {
                transactions.Add(new Transaction
                {
                    UserId = users[rand.Next(users.Count)].Id,
                    ProductId = products[rand.Next(products.Count)].Id,
                    Amount = rand.Next(100, 5000),
                    Status = statusValues[rand.Next(statusValues.Length)],
                    CreatedAt = DateTime.UtcNow.AddDays(-rand.Next(0, 365))
                });
            }

            db.Transactions.AddRange(transactions);
            db.SaveChanges();
        }
    }
}
