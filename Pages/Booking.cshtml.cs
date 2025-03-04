using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.DTO;
using testprojekt.Models;

namespace testprojekt.Pages
{
    public class BookingModel : PageModel
    {
        private readonly AppDbContext _context;

        public BookingModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BookingDto Booking { get; set; } = new BookingDto();

        public string Message { get; set; }

        public void OnGet(int? workspaceId)
        {
            if (workspaceId.HasValue)
            {
                Booking.WorkspaceId = workspaceId.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Booking.EndTime <= Booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "Время окончания бронирования должно быть позже начала.");
                return Page();
            }

            var workspace = _context.Workspaces.FirstOrDefault(ws => ws.Id == Booking.WorkspaceId);
            if (workspace == null)
            {
                ModelState.AddModelError("WorkspaceId", "Рабочее пространство не найдено.");
                return Page();
            }

            if (!workspace.IsAvailable)
            {
                ModelState.AddModelError("WorkspaceId", "Рабочее пространство уже забронировано.");
                return Page();
            }

            var booking = new Booking
            {
                StartTime = Booking.StartTime,
                EndTime = Booking.EndTime,
                WorkspaceId = Booking.WorkspaceId,
                UserId = 1 // В реальном приложении получайте UserId из контекста пользователя.
            };

            // Меняем статус рабочего пространства на "занято"
            workspace.IsAvailable = false;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            Message = "Бронирование успешно создано!";
            return Page();
        }
    }
}
