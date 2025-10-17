using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class PostMeta
    {
        [Key]
        public int MetaId { get; set; }

        public int? PostId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MetaKey { get; set; } = string.Empty;

        [Required]
        public string MetaValue { get; set; } = string.Empty;
    }
}
