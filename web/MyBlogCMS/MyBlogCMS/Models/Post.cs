using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlogCMS.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        // 🏷️ Tiêu đề bài viết
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài viết")]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        // ✏️ Mô tả ngắn
        [StringLength(500)]
        [Display(Name = "Mô tả ngắn")]
        public string? Description { get; set; }

        // 📄 Nội dung chi tiết (HTML)
        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = string.Empty;

        // 🖼️ Ảnh thumbnail (đường dẫn trong wwwroot/uploads/posts)
        [Display(Name = "Ảnh bìa")]
        public string? Thumbnail { get; set; }

        // 📂 File upload ảnh
        [NotMapped]
        [Display(Name = "Chọn ảnh bìa")]
        public IFormFile? ThumbnailFile { get; set; }

        // 🌆 Tự động tạo đường dẫn ảnh đầy đủ
        [NotMapped]
        public string ThumbnailPath =>
            string.IsNullOrEmpty(Thumbnail)
                ? "/images/no-image.jpg"
                : (Thumbnail.StartsWith("/") ? Thumbnail : "/uploads/posts/" + Thumbnail);

        // 📅 Ngày tạo
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 🕓 Ngày cập nhật
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        // 📚 Danh mục
        [Display(Name = "Chuyên mục")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // 👤 Tác giả
        [Display(Name = "Tác giả")]
        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }

        // 💬 Danh sách bình luận
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();

        // 🔗 Slug (URL thân thiện)
        [StringLength(200)]
        [Display(Name = "Đường dẫn (Slug)")]
        public string? Slug { get; set; }

        // ✅ Hiển thị trạng thái
        [Display(Name = "Công khai")]
        public bool IsPublished { get; set; } = true;
    }
}
