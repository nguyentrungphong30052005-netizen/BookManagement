using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BooksController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =====================================================
        // DANH SÁCH SÁCH
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();

            return View(books);
        }

        // =====================================================
        // XEM CHI TIẾT
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // =====================================================
        // HIỂN THỊ FORM THÊM SÁCH
        // =====================================================
        public IActionResult Create()
        {
            return View();
        }

        // =====================================================
        // THÊM SÁCH + UPLOAD ẢNH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Book book,
            IFormFile? ImageFile)
        {
            // Kiểm tra file ảnh
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string extension =
                    Path.GetExtension(ImageFile.FileName)
                        .ToLower();

                // Chỉ cho phép JPG, JPEG, PNG
                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png")
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Chỉ được upload file JPG hoặc PNG.");
                }

                // Giới hạn 5MB
                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Kích thước ảnh không được vượt quá 5MB.");
                }
            }

            if (ModelState.IsValid)
            {
                // Nếu người dùng có chọn ảnh
                if (ImageFile != null &&
                    ImageFile.Length > 0)
                {
                    // Đường dẫn thư mục lưu ảnh
                    string folderPath = Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "books");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Lấy phần mở rộng
                    string extension =
                        Path.GetExtension(ImageFile.FileName)
                            .ToLower();

                    // Tạo tên file mới
                    string fileName =
                        Guid.NewGuid().ToString()
                        + extension;

                    // Đường dẫn đầy đủ
                    string filePath =
                        Path.Combine(
                            folderPath,
                            fileName);

                    // Lưu ảnh vào server
                    using (var stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Lưu tên ảnh vào database
                    book.ImageFileName = fileName;
                }

                // Lưu sách
                _context.Books.Add(book);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // =====================================================
        // HIỂN THỊ FORM SỬA
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // =====================================================
        // SỬA SÁCH + ĐỔI ẢNH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Book book,
            IFormFile? ImageFile)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            // Lấy dữ liệu cũ
            var oldBook = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (oldBook == null)
            {
                return NotFound();
            }

            // Kiểm tra ảnh mới
            if (ImageFile != null &&
                ImageFile.Length > 0)
            {
                string extension =
                    Path.GetExtension(ImageFile.FileName)
                        .ToLower();

                // Chỉ cho phép JPG, JPEG, PNG
                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png")
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Chỉ được upload file JPG hoặc PNG.");
                }

                // Giới hạn 5MB
                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Kích thước ảnh không được vượt quá 5MB.");
                }
            }

            if (ModelState.IsValid)
            {
                // Nếu chọn ảnh mới
                if (ImageFile != null &&
                    ImageFile.Length > 0)
                {
                    string folderPath = Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "books");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(
                        oldBook.ImageFileName))
                    {
                        string oldImagePath =
                            Path.Combine(
                                folderPath,
                                oldBook.ImageFileName);

                        if (System.IO.File.Exists(
                            oldImagePath))
                        {
                            System.IO.File.Delete(
                                oldImagePath);
                        }
                    }

                    // Tạo tên ảnh mới
                    string extension =
                        Path.GetExtension(
                            ImageFile.FileName)
                            .ToLower();

                    string newFileName =
                        Guid.NewGuid().ToString()
                        + extension;

                    string newFilePath =
                        Path.Combine(
                            folderPath,
                            newFileName);

                    // Lưu ảnh mới
                    using (var stream =
                           new FileStream(
                               newFilePath,
                               FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Lưu tên ảnh mới
                    book.ImageFileName = newFileName;
                }
                else
                {
                    // Không chọn ảnh mới
                    // Giữ nguyên ảnh cũ
                    book.ImageFileName =
                        oldBook.ImageFileName;
                }

                _context.Books.Update(book);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // =====================================================
        // HIỂN THỊ FORM XÓA
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // =====================================================
        // XÓA SÁCH + XÓA ẢNH
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var book =
                await _context.Books.FindAsync(id);

            if (book != null)
            {
                // Xóa file ảnh khỏi server
                if (!string.IsNullOrEmpty(
                    book.ImageFileName))
                {
                    string imagePath =
                        Path.Combine(
                            _environment.WebRootPath,
                            "images",
                            "books",
                            book.ImageFileName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Xóa sách khỏi database
                _context.Books.Remove(book);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
