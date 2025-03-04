using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.DTO;
using testprojekt.Models;

namespace testprojekt.Pages.Workspaces
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // ������ ������� �����������, ���������� �� ���� ������.
        public IList<Workspace> WorkspacesFromDb { get; set; } = new List<Workspace>();

        // ������ ������� �����������, ���������� ����� API.
        public List<WorkspaceDto> WorkspacesFromApi { get; set; } = new List<WorkspaceDto>();

        // �� ��������� ���������� ������� ������������ �� ���� ������ ��� �����������.
        public IList<Workspace> Workspaces => WorkspacesFromDb;

        public async Task OnGetAsync()
        {
            // ��������� ������ �� ���� ������.
            WorkspacesFromDb = await _context.Workspaces.ToListAsync();

            // ��������� ������ ����� API.
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}/api/workspace");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    WorkspacesFromApi = JsonSerializer.Deserialize<List<WorkspaceDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
        }
    }
}

