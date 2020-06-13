using System;
using System.Threading.Tasks;
using cw5.Exceptions;
using Microsoft.AspNetCore.Http;

namespace cw5.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            switch (exception)
            {
                case BadRequestException _:
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = exception.Message
                    }.ToString());
                
                case ResourceNotFoundException _:
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = exception.Message
                    }.ToString());
                
                case BadLoginOrPasswordException _:
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = exception.Message
                    }.ToString());
                
                case ObjectAlreadyInDatabaseException _:
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    return context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = StatusCodes.Status409Conflict,
                        Message = exception.Message 
                    }.ToString());
                
                default:
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    return context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Unexpected exception"
                    }.ToString());
            }
        }
    }
}