namespace MyBlogCMS.Models
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Category> Categories { get; set; }
        public List<Post> FeaturedPosts { get; set; }
    }
}
