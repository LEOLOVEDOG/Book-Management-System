using Book_Management_System;
using Book_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// �]�w��Ʈw�s�u�r��
var connectionString = builder.Configuration.GetConnectionString("BookManagementSystem")
    ?? throw new InvalidOperationException("Connection string 'BookManagementSystem' not found.");

builder.Services.AddTransient<IEmailSender, FakeEmailSender>();


// ���U��Ʈw�W�U��
builder.Services.AddDbContext<BookManagementSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

// ���U Identity�A�ϥΦ۩w�q User �����M��Ʈw�W�U��
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<BookManagementSystemDbContext>() // �ϥΦ۩w�q�� DbContext
    .AddDefaultTokenProviders();

// �K�[ Razor Pages �M MVC ���
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// �t�m HTTP �޹D
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

app.UseAuthentication(); // �ҥΨ�������
app.UseAuthorization();  // �ҥα��v

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
