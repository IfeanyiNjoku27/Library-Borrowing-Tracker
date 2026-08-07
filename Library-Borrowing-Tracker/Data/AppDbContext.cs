using Microsoft.EntityFrameworkCore;
using Library_Borrowing_Tracker.Models;



namespace Library_Borrowing_Tracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Loans> Loans { get; set; }
    }
}
