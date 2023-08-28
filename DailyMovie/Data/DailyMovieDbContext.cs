using DailyMovie.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DailyMovie.Data
{
    public class DailyMovieDbContext : DbContext
    {
        public DailyMovieDbContext(DbContextOptions<DailyMovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
