using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Data;
using MyBlogCMS.Models;

namespace MyBlogCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly MyBlogContext _context;
        private readonly IWebHostEnvironment _env;

        public PostsController(MyBlogContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 🧭 Danh sách bài viết
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        // ➕ Tạo mới - GET
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // ➕ Tạo mới - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug SEO
                post.Slug = post.Title.ToLower().Replace(" ", "-");

                // Upload ảnh nếu có
                if (post.ThumbnailFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.ThumbnailFile.FileName);
                    string uploadPath = Path.Combine(_env.WebRootPath, "uploads", "posts");
                    Directory.CreateDirectory(uploadPath);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await post.ThumbnailFile.CopyToAsync(stream);

                    post.Thumbnail = fileName;
                }

                post.CreatedAt = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // ✏️ Sửa - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // ✏️ Sửa - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.PostId) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.PostId == id);
                if (existing == null) return NotFound();

                // Upload ảnh mới
                if (post.ThumbnailFile != null)
                {
                    string uploadPath = Path.Combine(_env.WebRootPath, "uploads", "posts");
                    Directory.CreateDirectory(uploadPath);

                    if (!string.IsNullOrEmpty(existing.Thumbnail))
                    {
                        string oldPath = Path.Combine(uploadPath, existing.Thumbnail);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.ThumbnailFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await post.ThumbnailFile.CopyToAsync(stream);

                    post.Thumbnail = fileName;
                }
                else
                {
                    post.Thumbnail = existing.Thumbnail;
                }

                post.Slug = post.Title.ToLower().Replace(" ", "-");
                post.UpdatedAt = DateTime.Now;

                _context.Update(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // ❌ Xóa - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null) return NotFound();

            return View(post);
        }

        // ❌ Xóa - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                if (!string.IsNullOrEmpty(post.Thumbnail))
                {
                    string path = Path.Combine(_env.WebRootPath, "uploads", "posts", post.Thumbnail);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
