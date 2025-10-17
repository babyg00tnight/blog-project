using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class PostViewModel
    {
        public int? PostId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? Thumbnail { get; set; }  // đường dẫn đã lưu trong DB

        public IFormFile? ThumbnailFile { get; set; } // file upload

        [Required]
        public string Content { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
    }
}
