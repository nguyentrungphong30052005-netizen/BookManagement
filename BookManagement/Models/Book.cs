using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [Display(Name = "Tên sách")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập tác giả")]
        [Display(Name = "Tác giả")]
        public string Author { get; set; } = "";

        [Display(Name = "Nhà xuất bản")]
        public string? Publisher { get; set; }

        [Range(0, 100000000, ErrorMessage = "Giá không hợp lệ")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Ngày xuất bản")]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }

        [Display(Name = "Ảnh sách")]
        public string? ImageFileName { get; set; }
    }
}
