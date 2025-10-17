using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Data;
using MyBlogCMS.Models;

namespace MyBlogCMS.Controllers
{
    public class PostsController : Controller
    {
        private readonly MyBlogContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(MyBlogContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // ===============================
        // 📋 INDEX - Danh sách bài viết (có tìm kiếm + phân trang)
        // ===============================
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 6; // ✅ 6 bài mỗi trang

            var postsQuery = _context.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            // 🔍 Tìm kiếm theo tiêu đề, mô tả, hoặc nội dung
            if (!string.IsNullOrEmpty(search))
            {
                postsQuery = postsQuery.Where(p =>
                    p.Title.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search)) ||
                    (p.Content != null && p.Content.Contains(search))
                );
            }

            // 🔢 Tổng số bài viết
            int totalPosts = await postsQuery.CountAsync();

            // 📄 Lấy dữ liệu cho trang hiện tại
            var posts = await postsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ✅ Gửi dữ liệu sang View
            ViewData["Search"] = search;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalPosts / (double)pageSize);

            return View(posts);
        }

        // ===============================
        // 🔍 DETAILS - Xem chi tiết bài viết
        // ===============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // ===============================
        // 💬 ADD COMMENT - chỉ cho user đăng nhập
        // ===============================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Vui lòng nhập nội dung bình luận!";
                return RedirectToAction("Details", new { id = postId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Bạn phải đăng nhập để bình luận.";
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                AuthorId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = postId });
        }

        // ===============================
        // 🆕 CREATE (GET)
        // ===============================
        [Authorize]
        public IActionResult Create()
        {
            var categories = _context.Categories?.ToList() ?? new List<Category>();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
            return View();
        }

        // ===============================
        // 🆕 CREATE (POST)
        // ===============================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                if (post.ThumbnailFile != null && post.ThumbnailFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "uploads/posts");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string fileName = Guid.NewGuid() + Path.GetExtension(post.ThumbnailFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await post.ThumbnailFile.CopyToAsync(stream);
                    }

                    post.Thumbnail = fileName;
                }

                post.CreatedAt = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var categories = _context.Categories?.ToList() ?? new List<Category>();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }
    }
}
