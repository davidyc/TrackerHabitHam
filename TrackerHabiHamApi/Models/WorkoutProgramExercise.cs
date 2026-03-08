using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerHabiHamApi.Models
{
    [Table("WorkoutProgramExercises")]
    public class WorkoutProgramExercise
    {
        [Key]
        public int Id { get; set; }

        public int WorkoutProgramId { get; set; }
        public WorkoutProgram WorkoutProgram { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public int Order { get; set; }
    }
}
