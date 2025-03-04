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

        // ������ ������������ ��� ����������� �� ��������
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public CommentsModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            // ��������� ����������� �� ����, �������� �� ���� (������� ����� �����)
            Comments = _context.Comments
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }
    }
}
