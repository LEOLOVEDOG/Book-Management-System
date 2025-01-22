using Book_Management_System;
using Book_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 設定資料庫連線字串
var connectionString = builder.Configuration.GetConnectionString("BookManagementSystem")
    ?? throw new InvalidOperationException("Connection string 'BookManagementSystem' not found.");

builder.Services.AddTransient<IEmailSender, FakeEmailSender>();


// 註冊資料庫上下文
builder.Services.AddDbContext<BookManagementSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

// 註冊 Identity，使用自定義 User 類型和資料庫上下文
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<BookManagementSystemDbContext>() // 使用自定義的 DbContext
    .AddDefaultTokenProviders();

// 添加 Razor Pages 和 MVC 支持
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 配置 HTTP 管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 啟用身份驗證
app.UseAuthorization();  // 啟用授權

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
