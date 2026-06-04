namespace Drajbot.Api.Interfaces
{
    public interface IAuditService
    {
        Task LogActionAsync(int? userId, string action, string details);
    }
}