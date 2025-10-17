using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Models;

namespace MyBlogCMS.Data
{
    // ✅ DbContext kế thừa từ IdentityDbContext để hỗ trợ quản lý User/Role
    public class MyBlogContext : IdentityDbContext<ApplicationUser>
    {
        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
        {
        }

        // Các DbSet riêng cho CMS
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Meta> Metas { get; set; } = null!;
        public DbSet<Source> Sources { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;
    }
}
