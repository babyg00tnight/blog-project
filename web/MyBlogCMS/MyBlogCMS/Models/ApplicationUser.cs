using Microsoft.AspNetCore.Identity;

namespace MyBlogCMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Thêm trường tùy chỉnh nếu cần
        public string? FullName { get; set; }
    }
}
