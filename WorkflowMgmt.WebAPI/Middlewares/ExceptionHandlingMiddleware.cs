using FluentValidation;
using System.Net;
using System.Text.Json;

namespace WorkflowMgmt.WebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = "Validation failed",
                    errors
                }));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = "An unexpected error occurred",
                    details = ex.Message
                }));
            }
        }
    }
}
