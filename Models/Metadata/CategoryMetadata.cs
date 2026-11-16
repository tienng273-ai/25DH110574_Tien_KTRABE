using System.ComponentModel.DataAnnotations;

namespace _25DH110574_Tien_KTRABE.Models
{
    public class CategoryMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự.")]
        public string CategoryName { get; set; }
    }
}