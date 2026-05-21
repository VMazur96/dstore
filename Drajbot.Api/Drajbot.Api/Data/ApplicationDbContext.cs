using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Drajbot.Api.Data
{
    // Parametri idu direktno ovde pored imena klase
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // Ovde ćemo dodavati sve tabele. Za sada imamo samo Users.
        public DbSet<User> Users { get; set; }
    }
}