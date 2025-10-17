using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Data;
using MyBlogCMS.Models;

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// 🚀 CẤU HÌNH DỊCH VỤ (SERVICES)
// ===================================================

// 1️⃣ Thêm MVC (Controller + View)
builder.Services.AddControllersWithViews();

// 2️⃣ Cấu hình DbContext (SQL Server)
builder.Services.AddDbContext<MyBlogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3️⃣ Cấu hình Identity (User + Role)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MyBlogContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

var app = builder.Build();

// ===================================================
// 🧩 SEED DỮ LIỆU MẪU (CHẠY 1 LẦN)
// ===================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MyBlogContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    context.Database.Migrate();

    // --- Seed Categories ---
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Tin tức" },
            new Category { Name = "Kinh dị" },
            new Category { Name = "Khoa học" }
        );
        await context.SaveChangesAsync();
    }

    // --- Seed Post mẫu ---
    if (!context.Posts.Any())
    {
        context.Posts.Add(new Post
        {
            Title = "Bài viết mẫu",
            Description = "Đây là bài viết mẫu.",
            Thumbnail = "/images/sample.jpg",
            Content = "Nội dung bài viết mẫu...",
            CategoryId = context.Categories.First().CategoryId,
            CreatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();
    }

    // --- Seed Role Admin ---
    const string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // --- Seed User Admin ---
    const string adminEmail = "gialongnghiem@gmail.com";
    const string adminPassword = "Fa271125";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}

// ===================================================
// ⚙️ MIDDLEWARE PIPELINE
// ===================================================
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

// ===================================================
// 🧭 CẤU HÌNH ROUTES
// ===================================================

// ✅ Route cho khu vực Admin (Area)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// ✅ Route mặc định cho site public
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ Route cho Razor Pages (Identity UI)
app.MapRazorPages();

// ✅ Endpoint test nhanh
app.MapGet("/ping", () => "pong");

// ===================================================
// 🚀 CHẠY ỨNG DỤNG
// ===================================================
app.Run();
