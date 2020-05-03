using System.Threading.Tasks;
using cw5.Services;
using Microsoft.AspNetCore.Http;

namespace cw5.Middleware
{
    public class ValidateIndexMiddleware
    {
        
        private readonly RequestDelegate _next;

        public ValidateIndexMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, IStudentsDbService studentsDbService)
        {
            if (!context.Request.Headers.ContainsKey("Index"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("No Index number entered");
                return;
            }

            var index = context.Request.Headers["Index"].ToString();

            if (!studentsDbService.CheckIfStudentExists(index))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized access");
                return;
            }

            if (_next != null)
            {
                await _next(context);
            }
        }
    }
}