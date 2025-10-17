using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TagName { get; set; } = string.Empty;
    }
}
