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

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, 100000000, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Ngày xuất bản")]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }
    }
}