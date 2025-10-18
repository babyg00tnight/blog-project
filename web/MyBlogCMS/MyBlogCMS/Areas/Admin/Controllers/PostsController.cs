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

        private string UploadFolder => Path.Combine(_env.WebRootPath, "uploads", "posts");

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
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", post.CategoryId);
                return View(post);
            }

            post.Slug = GenerateSlug(post.Title);
            post.CreatedAt = DateTime.Now;

            if (post.ThumbnailFile != null)
                post.Thumbnail = await SaveThumbnailAsync(post.ThumbnailFile);

            _context.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

            var existing = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.PostId == id);
            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", post.CategoryId);
                return View(post);
            }

            // Xử lý ảnh thumbnail
            if (post.ThumbnailFile != null)
            {
                if (!string.IsNullOrEmpty(existing.Thumbnail))
                    DeleteThumbnail(existing.Thumbnail);

                post.Thumbnail = await SaveThumbnailAsync(post.ThumbnailFile);
            }
            else
            {
                post.Thumbnail = existing.Thumbnail;
            }

            post.Slug = GenerateSlug(post.Title);
            post.UpdatedAt = DateTime.Now;

            _context.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ❌ Xóa - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // ❌ Xóa - POST (đồng bộ với view)
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(post.Thumbnail))
                DeleteThumbnail(post.Thumbnail);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 🔧 Helper: tạo slug SEO
        private static string GenerateSlug(string title)
        {
            return string.Join("-", title.ToLower().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        // 🔧 Helper: lưu ảnh thumbnail
        private async Task<string> SaveThumbnailAsync(IFormFile file)
        {
            Directory.CreateDirectory(UploadFolder);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(UploadFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        // 🔧 Helper: xóa ảnh cũ
        private void DeleteThumbnail(string fileName)
        {
            string path = Path.Combine(UploadFolder, fileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
