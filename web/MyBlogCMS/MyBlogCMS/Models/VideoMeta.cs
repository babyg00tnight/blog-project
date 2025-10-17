
using System;

namespace MyBlogCMS.Models
{
    public class VideoMeta
    {
        public int VideoMetaId { get; set; }
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        public string MetaKey { get; set; } = null!;
        public string? MetaValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
