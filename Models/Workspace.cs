using System.ComponentModel.DataAnnotations;

namespace testprojekt.Models
{
    public class Workspace
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerHour { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
