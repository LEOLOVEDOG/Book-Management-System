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

// �]�wUser�A��
builder.Services.AddScoped<IUserService, UserService>();

// �]�w JWT
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwtAuthentication(builder.Configuration);

// �]�w�l��A��
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, MailService>();

// �]�w Google OAuth
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


app.UseHttpsRedirection(); // �j�� HTTPS

app.UseCors("AllowSpecificOrigins"); // CORS ���ӭn���I�]�m�A�קK���ШD���D

app.UseRouting();

app.UseAuthentication(); // �����Ҩ����]�p�G���� JWT�BCookie ���ҵ��^

app.UseAuthorization(); // �A�ˬd�v��

app.MapControllers(); // �]�w���ѡ]�n�b���v����^

app.Run();
