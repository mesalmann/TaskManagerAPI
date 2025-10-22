//Kütüphane include ediyoruz.
using Microsoft.EntityFrameworkCore; //Entity Framework sınıflarına ulaşabilyoruz.
using TaskManagerAPI.Data;//Kendi oluşturduğumuz sınıflara ulaşabiliyoruz.
using TaskManagerAPI.Models;

//WebApllication sınıfından builder isimli bir nesne oluşturulur.
//Builder program başlamadan önce gerekli ayarları, servisleri ve konfigürasyonları toparlar.
// 'args' ise komut satırından gelen parametreleri temsil eder (örneğin --urls, --environment gibi).
var builder = WebApplication.CreateBuilder(args);

//gerekli servisler dependency injectiona kaydedilir.
// builder.Services.Add...() → o servisi bir kere kaydeder.
// uygulama boyunca ihtiyacı olan sınıflar bu servisi isteyince framework kutudan çıkarıp verir.
// tekrar tekrar new ile oluşturmaya gerek olmadan dışarıdan alınır.
// bu olaya Dependency Injection Container veya Service Container denir.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); //endpoint keşfi yapılır
builder.Services.AddSwaggerGen(); // swagger çalışması sağlanır. 

// Veritabanı nasıl yapılandırılacak burada belirlenir.
// Burada InMemory veritabanı seçildi; uygulama çalışırken RAM üzerinde geçici bir veritabanı oluşturur.
// SQL Server kullanmak isteseydik:
// builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("ConnectionString"));
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TasksDb"));


// builder.Build() çağrısı ile uygulama nesnesi oluşturulur.
// Bu noktada builder içine kaydedilen tüm servisler, konfigürasyonlar ve middleware'ler bir araya getirilir.
// Artık elimizde çalışan bir "app" (WebApplication) nesnesi vardır.
var app = builder.Build();

//VS kullanırken genellikle ASPNETCORE_ENVIRONMENT=Development olur.
//Canlı sunucuda Production olur.
//Uygulama geliştirme modundaysa (if env = Development) swagger açılır. Canlıda gözükmesini istemem.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//---------------------Middleware-------------------------//

//gelen HTTP isteklerini otomatik olarak HTTPS’e yönlendirir, böylece yanlış bağlantılar bile güvenli hale gelir.
app.UseHttpsRedirection();


//Controller dosyamı kullanılabilir hale getirir.
//Gelen requestler ve ilgili endpointler arasında bağlantı kurulmasını sağlar. 
app.MapControllers();

app.Run();
// artık hazır olan WebApplication nesnesini (yani app) başlatır.
// O ana kadar sadece yapı kurulmuştu (builder ile servisler, middleware’ler, routing).
// Run() çağrıldığında ASP.NET Core:
// Kestrel sunucusunu açar,
// belirtilen portlarda (örneğin 5000–7000) gelen HTTP/HTTPS isteklerini dinlemeye başlar,
// her isteği middleware zincirinden geçirip uygun controller’a gönderir.
