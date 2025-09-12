using Microsoft.EntityFrameworkCore;
using FintechBackend.Data;
using FintechBackend.Models;


namespace FintechBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}