using Drajbot.Api.Data;
using Drajbot.Api.Interfaces;
using Drajbot.Api.Models;

namespace Drajbot.Api.Services.Audits
{
    public class AuditService(ApplicationDbContext context) : IAuditService
    {
        public async Task LogActionAsync(int? userId, string action, string details)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details
            };

            context.AuditLogs.Add(log);
            await context.SaveChangesAsync();
        }
    }
}