using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Data;
using MyBlogCMS.Models;

namespace MyBlogCMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyBlogContext _context;

        public HomeController(MyBlogContext context)
        {
            _context = context;
        }

        // =====================================
        // 🏠 Trang chủ
        // =====================================
        public async Task<IActionResult> Index(string q, int page = 1, int pageSize = 9)
        {
            // Lấy danh mục
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Bài nổi bật (có ảnh)
            var featuredPosts = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !string.IsNullOrEmpty(p.Thumbnail))
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            // Query bài viết chính
            var query = _context.Posts
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p => p.Title.Contains(q) || p.Content.Contains(q));
                ViewData["q"] = q;
            }

            // Phân trang
            var totalPosts = await query.CountAsync();
            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gom tất cả vào ViewModel
            var vm = new HomeViewModel
            {
                Posts = posts,
                Categories = categories,
                FeaturedPosts = featuredPosts
            };

            // Truyền thêm dữ liệu phụ
            ViewData["TotalPosts"] = totalPosts;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            return View(vm);
        }
    }
}
