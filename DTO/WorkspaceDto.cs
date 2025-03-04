using System.ComponentModel.DataAnnotations;

namespace testprojekt.DTO
{
    public class WorkspaceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Вместимость должна быть положительным числом.")]
        public int Capacity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Цена за час должна быть положительной.")]
        public decimal PricePerHour { get; set; }

        public bool IsAvailable { get; set; }
    }
}
