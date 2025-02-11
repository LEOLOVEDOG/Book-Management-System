using Book_Management_System.Services;
using Book_Management_System_WebAPI;
using Book_Management_System_WebAPI.Interfaces;
using Book_Management_System_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("BookManagementSystem")
    ?? throw new InvalidOperationException("Connection string 'BookManagementSystem' not found.");

builder.Services.AddDbContext<BookManagementSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

// �]�w�}�����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5501")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // �p�G�ݭn�ǰe Cookie �α��v���Y�A�h�K�[����
    });
});

builder.Services.AddScoped<UserService>();

// Ū���]�w��
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<JwtService>();
builder.Services.AddJwtAuthentication(builder.Configuration);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
