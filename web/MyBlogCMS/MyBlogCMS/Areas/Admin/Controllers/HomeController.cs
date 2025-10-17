using Microsoft.AspNetCore.Mvc;
using MyBlogCMS.Data; // namespace của DbContext
using System.Linq;

namespace MyBlogCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly MyBlogContext _context;

        public HomeController(MyBlogContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalPosts = _context.Posts.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalUsers = _context.Users.Count();

            // Thống kê bài viết theo danh mục
            var postStats = _context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Count = _context.Posts.Count(p => p.CategoryId == c.CategoryId)
                })
                .ToList();

            ViewBag.PostStats = postStats;

            return View();
        }
    }
}
