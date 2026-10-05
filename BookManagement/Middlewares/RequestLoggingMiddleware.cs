using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookManagement.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Bắt đầu bấm giờ để tính milliseconds (ms)
            var stopwatch = Stopwatch.StartNew();

            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();

            // Chức năng 3: Chặn URL không hợp lệ trước khi vào Controller
            if (path == "/Book/Detail/0" || path == "/Book/Detail/-1")
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Book id khong hop le");
                return; // Ngừng request, không cho đi tiếp
            }

            // Cho request đi tiếp vào pipeline (Controller)
            await _next(context);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Chức năng 1 & 2: Ghi log thời gian, method, path, thời gian xử lý (ms) và Status Code
            Console.WriteLine($"[{time}] Method: {method} - Path: {path} ({elapsedMs}ms)");
            Console.WriteLine($"Status Code: {context.Response.StatusCode}");
        }
    }
}