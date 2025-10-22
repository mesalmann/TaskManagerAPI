using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    //Bu attribute, Controller’ın bir Web API Controller’ı olduğunu belirtir.
    //    - Gelen JSON otomatik olarak C# objesine çevrilir.
    [Route("api/[controller]")]
     //    URL adresini tanımlar: “api/task”
    //    [controller] ifadesi, sınıf adının “Task” kısmını otomatik alır (TaskController → /api/task).
    
    public class TaskController : ControllerBase // ControllerBase → sade, View’süz (sadece JSON dönen) API Controller’lar için temel sınıf.
    {
        private readonly AppDbContext _db;
        //    Dependency Injection (DI) ile veritabanı bağlantısı alınır.
        //    Yani her istekte EF Core’un AppDbContext örneği otomatik verilir.
        
        public TaskController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/task
        [HttpGet]
        //  Tüm görevleri listeleyen endpoint.
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetAll()
        {
            var items = await _db.Tasks
                .OrderByDescending(t => t.CreatedAt) // Görevleri tarihine göre sıralar (son eklenen en üstte).
                .Select(t => new TaskReadDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAt)) // Entity → DTO dönüşümü (sadece gerekli alanlar seçilir).
                .ToListAsync();

            return Ok(items); // 200 OK döner, gövdesinde görev listesi JSON olarak gelir.
        }

        // GET /api/task/{id}
        [HttpGet("{id:int}")] // Tek bir görevi ID’ye göre getirir.
        public async Task<ActionResult<TaskReadDto>> GetById(int id)
        {
            var t = await _db.Tasks.FindAsync(id);
            if (t is null) return NotFound(); // Eğer istenen ID’de görev yoksa 404 döner.
            return Ok(new TaskReadDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAt));
            //Varsa görev bilgilerini TaskReadDto formatında döner.
        }

        // POST /api/task
        [HttpPost] // Yeni görev oluşturur.
        public async Task<ActionResult<TaskReadDto>> Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            // DTO üzerindeki [Required], [StringLength] gibi validasyonlar kontrol edilir.
            var entity = new TaskItem
            {
                Title = dto.Title.Trim(), // Başlığı gereksiz boşluklardan temizler.
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                CreatedAt = DateTime.UtcNow // Oluşturulma tarihini sistem saatine göre ekler.
            };

            _db.Tasks.Add(entity);
            await _db.SaveChangesAsync();
              // Yeni görevi veritabanına ekler ve değişiklikleri kaydeder.

            var read = new TaskReadDto(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
        }

        // PUT /api/task/{id}
        [HttpPut("{id:int}")] //Var olan bir görevi tamamen günceller.
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
            // 204 No Content → güncelleme başarılı, geri veri dönmeye gerek yok.
        }

        // DELETE /api/task/{id}
        [HttpDelete("{id:int}")] // Görevi tamamen siler.
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Tasks.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
            // 204 No Content → silme işlemi başarılı.
        }

        // PATCH /api/task/{id}/complete
        [HttpPatch("{id:int}/complete")] //  Görevin tamamlanma durumunu değiştirir (true yapar).
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
              // Zaten tamamlandıysa bile 204 döner; idempotent bir endpoint’tir.
        }
    }
}
