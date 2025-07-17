
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TickiTackToe.Domain.Entities;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Infrastructure.Data
{
    public class TickDbContext(DbContextOptions<TickDbContext> options)
        : DbContext(options)
    {
        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonOptions = new JsonSerializerOptions();

            modelBuilder.Entity<Game>()
                .Property(g => g.Field)
                .HasColumnType("jsonb");
        }
    }
}
