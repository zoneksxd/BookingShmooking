using Microsoft.EntityFrameworkCore;
using testprojekt.Models;

namespace testprojekt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // Добавляем данные из другой БД (например, Comments)
        public DbSet<Comment> Comments { get; set; }
    }
}
