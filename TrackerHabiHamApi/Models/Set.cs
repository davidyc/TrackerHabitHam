using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerHabiHamApi.Models
{
    public class Set
    {
        [Key]
        public int Id { get; set; }

        public int WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public int Order { get; set; }

        public int? Reps { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? WeightKg { get; set; }
    }
}
