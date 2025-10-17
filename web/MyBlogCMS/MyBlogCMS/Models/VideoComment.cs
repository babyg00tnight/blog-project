
using System;

namespace MyBlogCMS.Models
{
    public class VideoComment
    {
        public int CommentId { get; set; }
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        public int? UserId { get; set; }
        public User? User { get; set; }
        public string Content { get; set; } = null!;
        public string Status { get; set; } = "visible";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
