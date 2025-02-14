using Book_Management_System_WebAPI;
using Book_Management_System_WebAPI.Interfaces;
using Book_Management_System_WebAPI.Models;
using Book_Management_System_WebAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("BookManagementSystem")
    ?? throw new InvalidOperationException("Connection string 'BookManagementSystem' not found.");

builder.Services.AddDbContext<BookManagementSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

// 設定開放網域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5501")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // 如果需要傳送 Cookie 或授權標頭，則添加此項
    });
});


builder.Services.AddScoped<IUserService, UserService>(); 

// 讀取設定值
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwtAuthentication(builder.Configuration);

// 設定郵件服務
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, MailService>();



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
