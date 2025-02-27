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

// 設定User服務
builder.Services.AddScoped<IUserService, UserService>();

// 設定 JWT
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwtAuthentication(builder.Configuration);

// 設定郵件服務
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, MailService>();

// 設定 Google OAuth
builder.Services.Configure<GoogleOAuth>(builder.Configuration.GetSection("GoogleOAuth"));
builder.Services.AddHttpClient<IAuthService, AuthService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection(); // 強制 HTTPS

app.UseCors("AllowSpecificOrigins"); // CORS 應該要早點設置，避免跨域請求問題

app.UseRouting();

app.UseAuthentication(); // 先驗證身份（如果有用 JWT、Cookie 驗證等）

app.UseAuthorization(); // 再檢查權限

app.MapControllers(); // 設定路由（要在授權之後）

app.Run();
