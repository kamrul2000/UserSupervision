using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserSupervision.Models;

namespace UserSupervision.Data
{
    public class AccuStockDbContext : DbContext
    {
        public AccuStockDbContext(DbContextOptions<AccuStockDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
