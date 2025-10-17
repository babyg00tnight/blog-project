using System;   // ✅ using ở đầu
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class PostComment
    {
        [Key]
        public int CommentId { get; set; }
        public int? PostId { get; set; }
        public int? UserId { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
