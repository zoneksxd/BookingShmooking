using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using testprojekt.DTO;

namespace testprojekt.Pages.Workspaces
{
    public class DetailsModel : PageModel
    {
        public WorkspaceDto Workspace { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}/api/workspace/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Workspace = JsonSerializer.Deserialize<WorkspaceDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            if (Workspace == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
