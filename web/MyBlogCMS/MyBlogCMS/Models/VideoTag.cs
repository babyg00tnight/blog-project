
namespace MyBlogCMS.Models
{
    public class VideoTag
    {
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
