using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// InMemory veritaban� (�imdilik ge�ici veriler i�in)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TasksDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// Basit seed: ba�ta �rnek iki g�rev olu�turur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Tasks.Any())
    {
        db.Tasks.AddRange(
            new TaskItem { Title = "Projeyi kur", Description = "Dotnet template ile olu�tur" },
            new TaskItem { Title = "Model yaz", Description = "TaskItem entity" }
        );
        db.SaveChanges();
    }
}

app.Run();
