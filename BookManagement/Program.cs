using BookManagement.Data;
using BookManagement.Middlewares; // <-- 1. Thêm namespace chứa middleware của bạn ở đây
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")
    ));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Cho phép truy cập file trong wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// --- 2. ĐĂNG KÝ MIDDLEWARE TỰ TẠO TẠI ĐÂY ---
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();