using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlogCMS.Data;

namespace MyBlogCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly MyBlogContext _context;
        public DashboardController(MyBlogContext context) => _context = context;

        public IActionResult Index()
        {
            ViewBag.PostCount = _context.Posts.Count();
            ViewBag.CategoryCount = _context.Categories.Count();
            return View();
        }
    }
}
