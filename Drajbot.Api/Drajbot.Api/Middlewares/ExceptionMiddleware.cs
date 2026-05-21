using System.Net;
using System.Text.Json;

namespace Drajbot.Api.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pusti zahtev da prođe dalje ka kontrolerima
                await next(context);
            }
            catch (Exception ex)
            {
                // Ako bilo gde u aplikaciji pukne greška, presrećemo je ovde!
                logger.LogError(ex, "Došlo je do interne greške: {ErrorMessage}", ex.Message);
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Vraćamo bezbednu, generičku poruku (ne odajemo strukturu servera)
            var response = new { message = "Došlo je do interne greške na serveru. Pokušajte ponovo kasnije." };
            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}