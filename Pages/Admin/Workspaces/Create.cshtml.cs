using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.Models;

namespace testprojekt.Pages.Admin.Workspaces
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Workspace Workspace { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Workspaces.Add(Workspace);
            await _context.SaveChangesAsync();
            Message = "–абочее пространство успешно добавлено!";
            return Page();
        }
    }
}
