
namespace MyBlogCMS.Models
{
    public class VideoCategory
    {
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
