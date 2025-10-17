namespace MyBlogCMS.Areas.Admin.Controllers
{
    // ViewModel dùng để hiển thị người dùng và vai trò trong Admin/Users
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }
}
