using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Data;

namespace MyBlogCMS.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly MyBlogContext _context;
        public CategoriesController(MyBlogContext context)
        {
            _context = context;
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }

        // GET: /Categories/Details/5  (hoặc /Categories/5)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var posts = await _context.Posts
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.Category = category;
            return View(posts);
        }
    }
}
