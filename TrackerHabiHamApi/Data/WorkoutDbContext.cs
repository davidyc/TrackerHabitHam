using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Data
{
    public class WorkoutDbContext : DbContext
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MuscleGroup> MuscleGroups { get; set; }
        public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
        public DbSet<WorkoutProgramExercise> WorkoutProgramExercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Set> Sets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MuscleGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<WorkoutProgram>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<WorkoutProgramExercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.WorkoutProgram)
                    .WithMany(p => p.Exercises)
                    .HasForeignKey(e => e.WorkoutProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Exercise)
                    .WithMany()
                    .HasForeignKey(e => e.ExerciseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Workout>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.HasOne(e => e.WorkoutProgram)
                    .WithMany(p => p.Workouts)
                    .HasForeignKey(e => e.WorkoutProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Workout)
                    .WithMany(w => w.Sets)
                    .HasForeignKey(e => e.WorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Exercise)
                    .WithMany()
                    .HasForeignKey(e => e.ExerciseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasOne(e => e.MuscleGroup)
                    .WithMany()
                    .HasForeignKey(e => e.MuscleGroupId)
                    .IsRequired(false);
            });
        }
    }
}
