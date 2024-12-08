using ChessCommon.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessCommon.Persistense
{
    public class ChessDBContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        public ChessDBContext(DbContextOptions<ChessDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().Property(q => q.Name).IsRequired();
            modelBuilder.Entity<Game>().Property(q => q.Moves).HasMaxLength(2000);
            base.OnModelCreating(modelBuilder);
        }

        //EntityFrameworkCore\Add-Migration InitialCreat
        //EntityFrameworkCore\Update-Database
        //The following configures EF to create a Sqlite database file in the
        //special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
        }

    }
}
