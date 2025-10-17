using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class Video
    {
        [Key]
        public int VideoId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Thumbnail { get; set; }   // link ảnh bìa

        public string? Content { get; set; }     // link nhúng hoặc nội dung chi tiết

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
