using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserSupervision.Models;

namespace UserSupervision.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SupervisionUser> SupervisionTable { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }


    }
}
