using System.ComponentModel.DataAnnotations;

namespace TrackerHabiHamApi.Models.Dto
{
    public class MounthWeight
    {
        [Key]
        public DateTime Date { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Weight { get; set; } = string.Empty;
    }
}


