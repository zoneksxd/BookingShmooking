using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testprojekt.Data;
using testprojekt.DTO;
using testprojekt.Models;
using System.Linq;

namespace testprojekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Проверяем, что время окончания бронирования позже времени начала
            if (bookingDto.EndTime <= bookingDto.StartTime)
            {
                ModelState.AddModelError("EndTime", "Время окончания бронирования должно быть позже начала.");
                return BadRequest(ModelState);
            }

            // Находим рабочее пространство по заданному WorkspaceId
            var workspace = _context.Workspaces.FirstOrDefault(ws => ws.Id == bookingDto.WorkspaceId);
            if (workspace == null)
                return NotFound("Рабочее пространство не найдено.");

            // Проверяем, доступно ли рабочее пространство для бронирования
            if (!workspace.IsAvailable)
                return BadRequest("Рабочее пространство уже забронировано.");

            // Здесь можно извлечь данные о пользователе из токена (например, через Claims), 
            // для демонстрации используем UserId = 1
            var booking = new Booking
            {
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                WorkspaceId = bookingDto.WorkspaceId,
                UserId = 1
            };

            // Меняем статус рабочего пространства на "занято"
            workspace.IsAvailable = false;

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return Ok("Бронирование успешно создано.");
        }
    }
}
