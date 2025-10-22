using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TaskController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetAll()
        {
            var items = await _db.Tasks
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TaskReadDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAt))
                .ToListAsync();

            return Ok(items);
        }

        // GET /api/task/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskReadDto>> GetById(int id)
        {
            var t = await _db.Tasks.FindAsync(id);
            if (t is null) return NotFound();
            return Ok(new TaskReadDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAt));
        }

        // POST /api/task
        [HttpPost]
        public async Task<ActionResult<TaskReadDto>> Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.Tasks.Add(entity);
            await _db.SaveChangesAsync();

            var read = new TaskReadDto(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
        }

        // PUT /api/task/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Title = dto.Title.Trim();
            entity.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            entity.IsCompleted = dto.IsCompleted;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/task/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Tasks.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH /api/task/{id}/complete
        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            if (!entity.IsCompleted)
            {
                entity.IsCompleted = true;
                await _db.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
