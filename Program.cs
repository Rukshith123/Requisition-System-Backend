using Microsoft.EntityFrameworkCore;
using RequisitionManagement.API.Data;
using RequisitionManagement.API.Repositories;
using RequisitionManagement.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RequisitionRepository>();
builder.Services.AddScoped<RequisitionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.EnsureCreated();

//     if (!db.Users.Any())
//     {
//         db.Users.Add(new RequisitionManagement.API.Models.User
//         {
//             Username = "admin",
//             Password = "admin", // replace with secure hashing in production
//             Role = "Manager"
//         });
//         db.SaveChanges();
//     }
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();