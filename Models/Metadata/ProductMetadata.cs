using System.ComponentModel.DataAnnotations;

namespace _25DH110574_Tien_KTRABE.Models
{
    public class ProductMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được quá 200 ký tự.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0.")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự.")]
        public string ProductDecription { get; set; }
    }
}