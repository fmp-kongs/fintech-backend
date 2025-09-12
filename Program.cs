using FintechBackend.Data;
using FintechBackend.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQLite for quick local dev + configure EF core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORS: allow Angular dev server (dev only)
builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            //policy.WithOrigins("http://localhost:4200")
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());

    }
);

var app = builder.Build();

// Create DB if not exists
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Name = "Alice", Email = "alice@example.com" },
            new User { Name = "Bob", Email = "bob@example.com" }
        );
        db.SaveChanges();
    }
}

// Middleware
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// environment - production
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
