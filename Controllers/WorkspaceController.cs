using Microsoft.AspNetCore.Mvc;
using testprojekt.Data;
using testprojekt.DTO;
using testprojekt.Models;
using System.Linq;

namespace testprojekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkspaceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var workspaces = _context.Workspaces
    .Select(ws => new WorkspaceDto
    {
        Id = ws.Id,
        Name = ws.Name,
        Description = ws.Description,
        Capacity = ws.Capacity,
        PricePerHour = ws.PricePerHour,
        IsAvailable = ws.IsAvailable
    })
    .ToList();

            return Ok(workspaces);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var workspace = _context.Workspaces.FirstOrDefault(ws => ws.Id == id);
            if (workspace == null)
                return NotFound();

            var dto = new WorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                Capacity = workspace.Capacity,
                PricePerHour = workspace.PricePerHour,
                IsAvailable = workspace.IsAvailable
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] WorkspaceDto workspaceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var workspace = new Workspace
            {
                Name = workspaceDto.Name,
                Description = workspaceDto.Description,
                Capacity = workspaceDto.Capacity,
                PricePerHour = workspaceDto.PricePerHour,
                IsAvailable = workspaceDto.IsAvailable
            };

            _context.Workspaces.Add(workspace);
            _context.SaveChanges();

            return Ok("Рабочее пространство создано.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WorkspaceDto workspaceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var workspace = _context.Workspaces.FirstOrDefault(ws => ws.Id == id);
            if (workspace == null)
                return NotFound();

            workspace.Name = workspaceDto.Name;
            workspace.Description = workspaceDto.Description;
            workspace.Capacity = workspaceDto.Capacity;
            workspace.PricePerHour = workspaceDto.PricePerHour;
            workspace.IsAvailable = workspaceDto.IsAvailable;

            _context.SaveChanges();
            return Ok("Рабочее пространство обновлено.");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var workspace = _context.Workspaces.FirstOrDefault(ws => ws.Id == id);
            if (workspace == null)
                return NotFound();

            _context.Workspaces.Remove(workspace);
            _context.SaveChanges();
            return Ok("Рабочее пространство удалено.");
        }
    }
}
