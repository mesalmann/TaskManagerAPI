using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class AppDbContext : DbContext //appDbContext sınıfını oluşturuyoruz. DbContextten kalıtım alıyoruz.
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //constructor method. 
        // EF Core’a veritabanı ayarlarını (örneğin InMemory veya SQL Server) iletir.
        // Bu constructor, Dependency Injection ile otomatik olarak çağrılır.
        
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        // Veritabanındaki "Tasks" tablosunu temsil eder.
        // TaskItem modeli üzerinden CRUD işlemleri yapılmasını sağlar.
    }
}
