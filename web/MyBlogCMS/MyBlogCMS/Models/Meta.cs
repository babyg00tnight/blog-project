using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class Meta
    {
        [Key]
        public int MetaId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } = string.Empty; // "post" | "video" | "user"

        public int EntityId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
    