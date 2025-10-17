using System.ComponentModel.DataAnnotations;

namespace MyBlogCMS.Models
{
    public class PostCategory
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
