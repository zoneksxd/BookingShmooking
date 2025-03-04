using System;
using System.ComponentModel.DataAnnotations;

namespace testprojekt.DTO
{
    public class BookingDto
    {
        [Required(ErrorMessage = "Начало бронирования обязательно.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Окончание бронирования обязательно.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Идентификатор рабочего пространства обязателен.")]
        public int WorkspaceId { get; set; }
    }
}
