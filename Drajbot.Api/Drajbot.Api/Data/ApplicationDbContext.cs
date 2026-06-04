using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Drajbot.Api.Data
{
    // Parametri idu direktno ovde pored imena klase
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // Ovde ćemo dodavati sve tabele. Za sada imamo samo Users.
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<SiteReview> SiteReviews { get; set; }
        public DbSet<UserWishlist> UserWishlists { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}