using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<MounthWeight> MounthWeights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MounthWeight>(entity =>
            {
                entity.HasKey(e => e.Date);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Weight).IsRequired().HasMaxLength(50);
            });
        }
    }
}


