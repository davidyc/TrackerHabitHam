using System.ComponentModel.DataAnnotations;

namespace TrackerHabiHamApi.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? MuscleGroupId { get; set; }
        public MuscleGroup? MuscleGroup { get; set; }
    }
}
