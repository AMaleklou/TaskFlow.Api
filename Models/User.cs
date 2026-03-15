namespace TaskFlow.Api.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key خودکار
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!; // بعداً پسورد را Hash می‌کنیم
    }
}
