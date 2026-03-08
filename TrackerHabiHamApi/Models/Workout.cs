using System.ComponentModel.DataAnnotations;

namespace TrackerHabiHamApi.Models
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        public int WorkoutProgramId { get; set; }
        public WorkoutProgram WorkoutProgram { get; set; } = null!;

        public DateOnly Date { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<Set> Sets { get; set; } = new List<Set>();
    }
}
