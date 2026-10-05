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
        // THÊM SÁCH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Book book,
            IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra người dùng có chọn ảnh hay không
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Đường dẫn thư mục lưu ảnh
                    string folderPath = Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "books");

                    // Nếu thư mục chưa tồn tại thì tạo
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Lấy phần mở rộng của ảnh
                    string extension =
                        Path.GetExtension(ImageFile.FileName);

                    // Tạo tên file ngẫu nhiên
                    string fileName =
                        Guid.NewGuid().ToString() + extension;

                    // Đường dẫn đầy đủ của file ảnh
                    string filePath =
                        Path.Combine(folderPath, fileName);

                    // Lưu ảnh vào thư mục
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

                // Thêm sách vào database
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
        // SỬA SÁCH
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

            if (ModelState.IsValid)
            {
                // Lấy dữ liệu cũ từ database
                var oldBook = await _context.Books
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (oldBook == null)
                {
                    return NotFound();
                }

                // Nếu người dùng chọn ảnh mới
                if (ImageFile != null && ImageFile.Length > 0)
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

                    // Lấy phần mở rộng ảnh mới
                    string extension =
                        Path.GetExtension(
                            ImageFile.FileName);

                    // Tạo tên ảnh mới
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

                    // Lưu tên ảnh mới vào database
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
        // XÓA SÁCH
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
                // Xóa file ảnh
                if (!string.IsNullOrEmpty(
                    book.ImageFileName))
                {
                    string imagePath =
                        Path.Combine(
                            _environment.WebRootPath,
                            "images",
                            "books",
                            book.ImageFileName);

                    if (System.IO.File.Exists(
                        imagePath))
                    {
                        System.IO.File.Delete(
                            imagePath);
                    }
                }

                // Xóa sách trong database
                _context.Books.Remove(book);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}