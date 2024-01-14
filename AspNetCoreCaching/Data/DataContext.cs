using AspNetCoreCaching.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreCaching.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<Item> Items { get; set; }
}