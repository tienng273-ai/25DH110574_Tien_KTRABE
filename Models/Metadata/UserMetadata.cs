using System.ComponentModel.DataAnnotations;

namespace _25DH110574_Tien_KTRABE.Models
{
    public class UserMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được quá 50 ký tự.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, ErrorMessage = "Mật khẩu không được quá 100 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        [StringLength(10, ErrorMessage = "Vai trò không được quá 10 ký tự.")]
        public string UserRole { get; set; }
    }
}