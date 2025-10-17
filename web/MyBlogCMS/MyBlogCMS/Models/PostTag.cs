using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class PostTag
    {
        [Key]
        public int Id { get; set; }

        public int? PostId { get; set; }
        public int? VideoId { get; set; }
        public int? TagId { get; set; }
    }
}
