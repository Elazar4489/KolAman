using CommandSystemDB.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandSystemDB.Settings
{
    public class CommandSystemDbContext : DbContext
    {
        public CommandSystemDbContext(DbContextOptions<CommandSystemDbContext> options) : base(options) { }
        public DbSet<Alert> alerts => Set<Alert>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alert>()
                .HasKey(a => a.alert_id); 
        }
    }
}
