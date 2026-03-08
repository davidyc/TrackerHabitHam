using System.ComponentModel.DataAnnotations;

namespace TrackerHabiHamApi.Models
{
    public class WorkoutProgram
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WorkoutProgramExercise> Exercises { get; set; } = new List<WorkoutProgramExercise>();
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}
