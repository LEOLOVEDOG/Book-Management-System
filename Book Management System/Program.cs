using Book_Management_System.Models;
using Book_Management_System.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 設定資料庫連線字串
var connectionString = builder.Configuration.GetConnectionString("BookManagementSystem")
    ?? throw new InvalidOperationException("Connection string 'BookManagementSystem' not found.");

//builder.Services.AddTransient<IEmailSender, FakeEmailSender>();
builder.Services.AddTransient<IEmailSender, EmailSender>();



// 註冊資料庫上下文
builder.Services.AddDbContext<BookManagementSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

// 註冊 Identity，使用自定義 User 類型和資料庫上下文
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<BookManagementSystemDbContext>() 
    .AddDefaultTokenProviders();

//身分驗證---Start
builder.Services.Configure<IdentityOptions>(options => {
    //options.Password.RequireDigit = true;
    //options.Password.RequireLowercase = true;
    //options.Password.RequireNonAlphanumeric = true;
    //options.Password.RequireUppercase = true;
    //options.Password.RequiredLength = 8;
    //options.Password.RequiredUniqueChars = 1;

    ////鎖幾分
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    ////輸錯幾次鎖
    //options.Lockout.MaxFailedAccessAttempts = 3;
    //options.Lockout.AllowedForNewUsers = true;

    //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    //驗證唯一信箱
    options.User.RequireUniqueEmail = true;

    //需不需要驗證信箱
    options.SignIn.RequireConfirmedEmail = true;
});
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
//身分驗證---End


// 設定 FaceBook Google 身份驗證
builder.Services.AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


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

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
