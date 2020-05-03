using System.IO;
using System.Text;
using System.Threading.Tasks;
using cw5.Services;
using Microsoft.AspNetCore.Http;

namespace cw5.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingService service)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                var path = context.Request.Path;
                var method = context.Request.Method;
                var queryString = context.Request.QueryString.ToString();

                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1204, true);
                var bodyString = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

               service.LogToFile(path, method, queryString, bodyString);
               
            }

            if (_next != null)
            {
                await _next(context);
            }
        }
    }
    
}