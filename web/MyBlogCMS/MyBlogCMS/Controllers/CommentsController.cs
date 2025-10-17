using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogCMS.Data;
using MyBlogCMS.Models;

namespace MyBlogCMS.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(MyBlogContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📝 POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Bình luận không được để trống.";
                return RedirectToAction("Details", "Posts", new { id = postId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge(); // Yêu cầu đăng nhập

            var comment = new Comment
            {
                PostId = postId,
                Content = content.Trim(),
                AuthorId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Posts", new { id = postId });
        }

        // ❌ XÓA: chỉ cho phép chủ comment hoặc admin
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.Include(c => c.Author).FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (comment.AuthorId != currentUser.Id && !isAdmin)
                return Forbid(); // Không được xóa comment người khác

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Posts", new { id = comment.PostId });
        }
    }
}
