using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }   // cho phép null vì số điện thoại có thể không bắt buộc

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;   // ví dụ: "member", "admin"

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
