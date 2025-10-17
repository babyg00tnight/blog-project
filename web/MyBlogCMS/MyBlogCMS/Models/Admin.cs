using System;

namespace MyBlogCMS.Models
{
    public class Admin
    {
        public int AdminId { get; set; }   // Mã admin
        public string Name { get; set; }   // Tên
        public string Phone { get; set; }  // SĐT
        public string Email { get; set; }  // Email
        public string PasswordHash { get; set; } // Mật khẩu (hash)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
