using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using testprojekt.Data;
using testprojekt.Models;

namespace testprojekt.Pages
{
    public class CommentsModel : PageModel
    {
        private readonly AppDbContext _context;

        // —писок комментариев дл€ отображени€ на странице
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public CommentsModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            // »звлекаем комментарии из базы, сортиру€ по дате (сначала самые новые)
            Comments = _context.Comments
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }
    }
}
