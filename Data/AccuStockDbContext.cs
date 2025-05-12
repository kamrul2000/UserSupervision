using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserSupervision.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UserSupervision.Data
{
    public class AccuStockDbContext : DbContext
    {
        public AccuStockDbContext(DbContextOptions<AccuStockDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserBillStatus> UserBillStatuses { get; set; }

    }
}
