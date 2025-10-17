using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class Source
    {
        [Key]
        public int SourceId { get; set; }
        public int? PostId { get; set; }
        public int? VideoId { get; set; }
        public string? Content { get; set; }
    }
}
